using System;

namespace KnoxGameStudios
{
    [Serializable]
    public class PhotonStatus
    {
        public string PlayerName { get; private set; }
        public int Status { get; private set; }
        public string Message { get; private set; }

        public PhotonStatus(string name, int status, string message)
        {
            PlayerName = name;
            Status = status;
            Message = message;
        }
    }
}