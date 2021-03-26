using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MsTeamStressTest.Models;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace MsTeamStressTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Models.AppSettings appSettings;
            EnvironmentModel config;
            DataTable dtTblSeendMeetingLink = Helper.initDTsendMeetingLinkReport();
            DataTable dtTblGoToAccount = Helper.initDTdtTblGoToAccountReport();
            var lstSGoToAccountkRslt = new List<GoToAccountResult>();

            appSettings = Helper.ParseAppSettings();
            config = Helper.ParseEnvironmentConfig(appSettings.Env);
            string csvPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\SampleData\{config.CsvData}";

            //Load MSID Data
            var yourData = File.ReadAllLines(csvPath)
                   .Skip(1)
                   .Select(x => x.Split(','))
                   .Select(x => new
                   {
                       MsteamId = x[0],
                       Email = x[1]
                   });

            var yourList = yourData.ToList();
            var myList = yourList.ToList();

            for (int i = 0; i < appSettings.TotalIterations - 1; i++)
            {
                myList.AddRange(yourList.ToList());
            }

            int runningThreadCount = 0;
            int totalDataProcessed = 0;
            int totalDataCount = myList.Count();

            Console.WriteLine($"CsvPath: {csvPath}");
            Console.WriteLine($"Processing the data.... Max Thread: {appSettings.Thread}, number of data: {totalDataCount}");

            Parallel.ForEach(myList, new ParallelOptions { MaxDegreeOfParallelism = appSettings.Thread },
                data =>
                {
                    string threadId = (Thread.CurrentThread.ManagedThreadId).ToString();
                    try
                    {
                        Interlocked.Increment(ref runningThreadCount);
                        Console.WriteLine($"ThreadId: {threadId} started. Number of running thread: {runningThreadCount} out of {appSettings.Thread}.");
                        Console.WriteLine($"ThreadId: {threadId} processing for: {data.MsteamId}, {data.Email}.");
                        List<SendMeetingLinkResult> smlRslt = Helper.SendMeetingLink(data.MsteamId, data.Email, config);
                        foreach (var rslt in smlRslt)
                        {
                            lock (dtTblSeendMeetingLink)
                            {
                                DataRow row = dtTblSeendMeetingLink.NewRow();
                                row["MSTEAMS_ID"] = rslt.MSTEAM_ID;
                                row["EMAIL"] = rslt.EMAIL;
                                row["RESPONS_MESSAGE"] = rslt.SM_RESPONS;
                                row["THREAD_ID"] = threadId;
                                row["MACHINE_NAME"] = rslt.MACHINE_NAME;
                                row["SEND_MEETING_LINK_ELAPSED_TIME_IN_SECONDS"] = rslt.SEND_MEETING_LINK_TIME;
                                row["TOKEN_ELAPSED_TIME_IN_SECONDS"] = rslt.GET_TOKEN_TIME;
                                dtTblSeendMeetingLink.Rows.Add(row);
                            }
                        }

                        List<GoToAccountResult> gtaRslt = Helper.GoToAccount(data.MsteamId, data.Email, config);
                        foreach (var rslt1 in gtaRslt)
                        {
                            lock(dtTblGoToAccount)
                            {
                                DataRow row = dtTblGoToAccount.NewRow();
                                row["MSTEAMS_ID"] = rslt1.MSTEAM_ID;
                                row["EMAIL"] = rslt1.EMAIL;
                                row["STATUS_CODE"] = rslt1.GO_TO_ACCOUNT_RESPONSES;
                                row["THREAD_ID"] = threadId;
                                row["MACHINE_NAME"] = rslt1.MACHINE_NAME;
                                row["GO_TO_ACCOUNT_ELAPSED_TIME_IN_SECONDS"] = rslt1.GO_TO_ACCOUNT__TIME;
                                row["TOKEN_ELAPSED_TIME_IN_SECONDS"] = rslt1.GET_TOKEN_TIME;
                                dtTblGoToAccount.Rows.Add(row);
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        Interlocked.Increment(ref totalDataProcessed);
                        Interlocked.Decrement(ref runningThreadCount);
                        Console.WriteLine($"ThreadId: {threadId} finished. Number of running thread: {runningThreadCount} out of {appSettings.Thread}. Total data processed: {totalDataProcessed} out of {totalDataCount}.");
                    }
                });
            StringBuilder meetingLinkData = Helper.ConvertDataTableToCsvFile(dtTblSeendMeetingLink);
            Helper.SaveData(meetingLinkData, config.ResultMeetinglink);

            StringBuilder goToAccountData = Helper.ConvertDataTableToCsvFile(dtTblGoToAccount);
            Helper.SaveData(goToAccountData, config.ResultGoAccount);
            Console.WriteLine("Completed. Press any key to exit the app.");
            Console.ReadLine();
        }
    }
}
