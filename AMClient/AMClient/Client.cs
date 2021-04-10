namespace AMClient
{
	public class Client
	{
		public Client(string PrivateKey)
		{
            this.PrivateKey = PrivateKey;
        }

		public string PrivateKey { get; private set; }
		public bool Connect() => true;
        public void Disconnect() { }
		public void EnableAutoReconnect() { }
        public bool CheckPrivateKey() => true;
		public int ProductCount(int productId, int productModeId) => int.MaxValue;
		public string GetOption(string optionName) => string.Empty;
        public bool MaxSessionReached(int productId) => false;
        public bool CrashReport(string ProductName, string Details) => false;
		public bool StartSession(int productId)=>  true;
		public void StopSession() { }
        public bool MultiIPCheck(int productId) =>  false;
	}
}
