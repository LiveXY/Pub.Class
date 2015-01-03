using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ispring_samples
{
    class converter
    {
        static void Help()
        {
            Console.WriteLine("iSpring SDK Multithreaded example");
            Console.WriteLine("Syntax:");
            String filePath = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            String fileName = filePath.Substring(filePath.LastIndexOf("/") + 1);
            Console.WriteLine(fileName + " " + "<ppt> <swf> [<ppt> <swf> ...]");
        }

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Help();
                Environment.Exit(0);
            }

            if ((args.Length % 2) != 0)
            {
                Console.WriteLine("Invalid number of arguments.");
                Help();
                Environment.Exit(-1);
            }

            int numberOfPresentations = args.Length / 2;

            Thread[] conversionThreads = new Thread[numberOfPresentations];

            // initialize threads
            for (int presentationIndex = 0; presentationIndex < numberOfPresentations; ++presentationIndex)
            {
                String pptName = args[presentationIndex * 2];
                String swfName = args[presentationIndex * 2 + 1];

                Console.WriteLine("Convert \"" + pptName + "\" to \"" + swfName + "\"");
                iSpringConverter converter = new iSpringConverter(pptName, swfName);
                Thread thread = new Thread(converter.StartConversion);

                ApartmentState astate = thread.GetApartmentState();
                thread.SetApartmentState(ApartmentState.STA);

                conversionThreads[presentationIndex] = thread;
            }

            // Start conversion
            for (int presentationIndex = 0; presentationIndex < numberOfPresentations; ++presentationIndex)
            {
                conversionThreads[presentationIndex].Start();
            }

            // wait for threads
            for (int presentationIndex = 0; presentationIndex < numberOfPresentations; ++presentationIndex)
            {
                conversionThreads[presentationIndex].Join();
            }
        }
    }
}
