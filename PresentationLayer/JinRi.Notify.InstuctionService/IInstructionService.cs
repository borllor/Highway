using System.Collections.Generic;
using System.ServiceModel;
using JinRi.Notify.DTO;

namespace JinRi.Notify.InstuctionService
{
    [ServiceContract]
    public interface IInstructionService
    {
        [OperationContract]
        bool RegisterServer(BeatMessage beatMessage);

        [OperationContract]
        BeatResult HeartBeat(BeatMessage beatMessage);

        [OperationContract]
        bool CreateTask(TaskMessage taskMessage);

        [OperationContract]
        List<TaskMessage> GetTaskMessageList();

        [OperationContract]
        List<string> GetAllServers();
        
    }


}
