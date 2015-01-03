using System;
using System.Collections.Generic;
using System.Text;
using iSpring;

namespace ispring_samples {
    class iSpringConverter {
        private String m_pptFileName;
        private String m_swfFileName;
        private int m_index = 0;
        private bool m_dataProcessingStarted = false;
        private static int m_currentThreadIndex = 0;
        PresentationConverter m_pptConverter;

        public iSpringConverter(String pptFileName, String swfFileName) {
            m_pptFileName = pptFileName;
            m_swfFileName = swfFileName;
            lock (this) {
                m_index = ++m_currentThreadIndex;
            }
            try {
            } catch (Exception e) {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        public void StartConversion() {
            // if ((m_pptConverter != null) && (m_pptConverter.PresentationOpened))
            //{
                try {
                    LogLine("Initializing");
                    m_pptConverter = new PresentationConverter();

                    // open presentation
                    LogLine("Opening presentation " + m_pptFileName);
                    m_pptConverter.OpenPresentation(m_pptFileName);
                    LogLine("Presentation " + m_pptFileName + " opened successfully");

                    // set up event handlers to display conversion progress
                    m_pptConverter.OnStartProcessingData += new _iSpringEvents_OnStartProcessingDataEventHandler(pptConverter_OnStartProcessingData);
                    m_pptConverter.OnFinishProcessingData += new _iSpringEvents_OnFinishProcessingDataEventHandler(pptConverter_OnFinishProcessingData);
                    m_pptConverter.OnSlideProgressChanged += new _iSpringEvents_OnSlideProgressChangedEventHandler(pptConverter_OnSlideProgressChanged);


                    System.Threading.Thread.Sleep(10);

                    // convert presentation
                    LogLine("Generating SWF file " + m_swfFileName);
                    m_pptConverter.GenerateSolidPresentation(m_swfFileName, null, null);
                    LogLine("SWF File " + m_swfFileName + " generated successfully");
                } catch (Exception e) {
                    Console.WriteLine("Error: " + e.Message);
                }

            //}
        }

        void pptConverter_OnFinishProcessingData() {
            m_dataProcessingStarted = false;
        }

        void pptConverter_OnStartProcessingData() {
            m_dataProcessingStarted = true;
        }

        void pptConverter_OnSlideProgressChanged(int slideIndex, int totalSlides) {
            if (m_dataProcessingStarted) {
                LogLine("Processing slide " + (slideIndex + 1) + " / " + totalSlides);
            }
        }

        private void LogLine(String message) {
            Console.WriteLine(m_index + "> " + message);
        }
    }
}
