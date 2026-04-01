using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace WMSInterface
{
    public interface WaveComputingCallBack
    {
        [System.ServiceModel.OperationContract(IsOneWay = true)]
        void KeepAlive();

        [OperationContract(IsOneWay = true)]
        void Process(Int32 nowProcess);

        [OperationContract(IsOneWay = true)]
        void ProcessLine(String orderLine, Int32 maxNowProcess, Int32 lineProcess);

        [OperationContract(IsOneWay = true)]
        void Init(String orderLine, Int32 maxLineProcess);

        [OperationContract()]
        void Finish(String orderLine);
    }
}
