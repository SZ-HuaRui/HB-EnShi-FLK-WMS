using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Synthesis; 
using System.Speech;
using System.Speech.Recognition;
using System.Collections.Concurrent;

namespace CWCS.Core.Library
{
    public class QueueSpeech
    {
        static SpeechSynthesizer voice = new SpeechSynthesizer();
        //private static Queue<string> ListQueue = new Queue<string>();
        private static ConcurrentQueue<string> ListQueue = new ConcurrentQueue<string>();
        public static void AddQueue(string msg) //入列  
        {
            if (!ListQueue.Contains(msg) && ListQueue.Count < 7)
                ListQueue.Enqueue(msg);
        }

        public static void Start()//启动  
        {
            Thread thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Start();
        }

        private static void threadStart()
        {
            while (true)
            {
                if (ListQueue.Count > 0)
                {
                    try
                    {
                        ScanQueue();
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    //没有任务，休息5秒钟  
                    Thread.Sleep(5000);
                }
            }
        }

        private static void ScanQueue()//出列
        {
            try
            {
                string msg = "";
                while (ListQueue.TryDequeue(out msg))
                {
                    voice.SpeakAsync(msg);
                }
            }
            catch { }
        }


    }
}
