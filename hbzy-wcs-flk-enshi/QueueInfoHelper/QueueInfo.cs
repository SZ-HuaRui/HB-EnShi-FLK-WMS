using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Synthesis; 
using System.Speech;
using System.Speech.Recognition;

namespace QueueInfoHelper
{
    public  class QueueInfo
    {
       static SpeechSynthesizer voice = new SpeechSynthesizer();
        private static Queue<string> ListQueue = new Queue<string>();
        public static void AddQueue(string text) //入列  
        {
            ListQueue.Enqueue(text);
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
                    //没有任务，休息3秒钟  
                    Thread.Sleep(3000);
                }
            }
        }

        private static void ScanQueue()
        {
            while (ListQueue.Count > 0)
            {
                try
                {
                    //从队列中取出  
                    string queueinfo = ListQueue.Dequeue();
                    voice.SpeakAsync(queueinfo);
                    //Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


    }
}
