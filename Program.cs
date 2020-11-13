using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.ServiceModel.Security;
using Opc.Ua;
using Opc.Ua.Configuration;
using Org.BouncyCastle.Utilities;

namespace Quickstarts.ConsoleReferenceClient
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("OPC UA Console Reference Client");

            try
            {
                // Define the UA Client application
                ApplicationInstance application = new ApplicationInstance();
                application.ApplicationName = "Quickstart Console Reference Client";

                application.ApplicationType = ApplicationType.Client;

                // load the application configuration.
                application.LoadApplicationConfiguration("ConsoleReferenceClient.Config.xml", false).Wait();
                // check the application certificate.
                application.CheckApplicationInstanceCertificate(false, 0).Wait();

                // create the UA Client object and connect to configured server.
                UAClient uaClient = new UAClient(application.ApplicationConfiguration);

                Console.WriteLine("Please specify your Server IP:");
                uaClient.ServerUrl = Console.ReadLine();

                if (uaClient.Connect())
                {
                    // Run tests for available methods.
                    uaClient.ReadNodes();
                    //uaClient.WriteNodes();
                    var tempcollection = uaClient.Browse(ObjectIds.Server);
                    NodeId lasttempnode = ObjectIds.Server;

                   Stack<NodeId> lastnode = new Stack<NodeId>();
                    bool x = true;
                    while (x)
                    {
                        int number;
                        if(lastnode.Count != 0)
                            Console.WriteLine("999      backwards");

                        Console.WriteLine("What Number would you like to check? ");
                        string index = Console.ReadLine();
                        bool isNumeric = int.TryParse(index, out number);
                        if(tempcollection != null)
                        if (((!isNumeric || number > tempcollection.Count) && (number != 999 ) && (number != 998)) || (number == 999 && lastnode.Count == 0))
                        {
                            Console.WriteLine("Only displayed Numbers are supported. Try again!");
                            continue;
                        }
                        if (number == 999)
                        {
                            lasttempnode = lastnode.Pop();
                            tempcollection = uaClient.Browse(lasttempnode);
                            continue;
                        }
                        /*if (number == 998) {
                            
                            Console.WriteLine("What attribute would you like to check? ");
                            string attributindex = Console.ReadLine();
                            uint number2;
                            bool isNumeric2 = uint.TryParse(attributindex, out number2);
                            if (isNumeric2)
                                uaClient.readNode(lastnode.Peek());
                            continue;
                        }*/
                        lastnode.Push(lasttempnode);
                        lasttempnode = (NodeId)tempcollection[number].NodeId;
                        tempcollection = uaClient.Browse((NodeId)tempcollection[number].NodeId);

                    }
                    //uaClient.SubscribeToDataChanges();
                    // Wait for some DataChange notifications from MonitoredItems
                    //System.Threading.Thread.Sleep(20000);

                    uaClient.Disconnect();
                }
                else
                {
                    Console.WriteLine("Could not connect to server! Restart Program.");
                }

                Console.WriteLine("\nProgram ended.");
                Console.WriteLine("Press any key to finish...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}