using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using CsvHelper;
using gitdowntonight.Exceptions;
using gitdowntonight.Models;
using gitdowntonight.Services.Interfaces;
using Microsoft.Extensions.Options;
using Serilog;

namespace gitdowntonight.Services.DBImpl
{
    public class TextDatabaseResultHandlingService : IHandleResults
    {
        private readonly MyOptions _options;
        private readonly ILogger _log = Log.ForContext<TextDatabaseResultHandlingService>();
        private readonly ISortContributors _sortContributors;
        private const int LockedFileTimeout = 1000; // One Second

        public TextDatabaseResultHandlingService(IOptionsMonitor<MyOptions> options, ISortContributors sortContributors)
        {
            _options = options.CurrentValue;
            _sortContributors = sortContributors;
        }

        /// <summary>
        /// Writes the results to a csv file.
        /// </summary>
        /// <param name="contributions"> The list of Contributions</param>
        public void Handle(List<Contribution> contributions)
        {
            //Placing it in the same dir as we are running from makes it simpler.
            //Normally would have this in configuration, but pc -> mac paths are a p.i.a
            var filePath = $"{Environment.CurrentDirectory}\\{_options.Organization}.csv";
            //checks if txt file exist
            if (!File.Exists(filePath))
            {
                _log.Debug("Creating database file");
                //create if not
                var file = File.Create(filePath);
                file.Close();
            }

            var records = ReadCSV(filePath);
            // Checks if the results in the file are the same as what it has
            if (CompareRecords(records, contributions))
            {
                //Results are already in the file yay, return
                _log.Information("No changes to the DB");
                return;
            }

            //Overwrite results
            WriteResults(contributions, filePath);
        }

        private List<Contribution> ReadCSV(string filePath)
        {
            //With using it auto-magically cleans up after itself yay
            //It also scopes so you don't accidentally use a freed object
            try
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    if (fileStream.Length <= 0)
                    {
                        return null;
                    }
                    using (var reader = new StreamReader(fileStream))
                    {
                        using (var csv = new CsvReader(reader))
                        {
                            _log.Debug("Reading database file");

                            return csv.GetRecords<Contribution>().ToList();
                        }
                    }
                }
            }
            catch (IOException e)
            {
                if (wasLockingIssue(e))
                {
                    _log.Error("DB File was locked when reading");
                    //Handle Locked file
                    Thread.Sleep(LockedFileTimeout);
                    return ReadCSV(filePath);
                }
                //Else it was not a locking issue
                //Probably file permissions
                throw new GitDownTonightIOException("Failed to read CSV", e);
            }
        }

        private bool wasLockingIssue(IOException e)
        {  
            //Magic code from StackOverflow to see if this was because it was a locked file
            //https://stackoverflow.com/questions/1304/how-to-check-for-file-lock/20623302#20623302
            var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
            return errorCode == 32 || errorCode == 33;
        }

        private bool CompareRecords(List<Contribution> records, List<Contribution> newContributions)
        {
            if (records == null)
            {
                return false;
            }
            var sortedRecords = _sortContributors.Sort(records.ToList());
            //Use sequence equal to check theyre in the same order as well as have the same objects
           return newContributions.SequenceEqual(sortedRecords);
        }

        private void WriteResults(List<Contribution> newContributions, string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                {
                    using (var csv = new CsvWriter(writer))
                    {
                        _log.Debug("Writing Records");
                        csv.WriteRecords(newContributions);
                    }
                }
            }
            catch (IOException e)
            {
                if (wasLockingIssue(e))
                {
                    _log.Error("DB File was locked when writing");
                    //Handle Locked file
                    Thread.Sleep(LockedFileTimeout);
                    WriteResults(newContributions, filePath);
                }
                //Else it was not a locking issue
                //Probably file permissions
                throw new GitDownTonightIOException("Failed to write CSV", e);
            }
        }
    }
}