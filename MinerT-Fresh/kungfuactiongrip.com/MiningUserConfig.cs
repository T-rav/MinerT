namespace MinerT.kungfuactiongrip.com
{
    /// <summary>
    /// Used for a mining pool
    /// </summary>
    public class MiningPool
    {
        public string Pool { get; set; }

        public bool IsManagedPool { get; set; }

    }
    /// <summary>
    /// Holds mining balance data ;)
    /// </summary>
    public class MiningBalance
    {
        public double Balance { get; set; }
	
	    public string BalanceCurrency { get; set; }
	
	    public string BalanceTs { get; set; }
	
	    public int BalanceRank { get; set; }

        public string Wallet;

        public override string ToString()
        {
            return Balance + " " + BalanceCurrency.ToUpper() + " @ " + BalanceTs;
        }
    }

    /// <summary>
    /// TO for fetching, updating user info
    /// </summary>
    public class MiningUserConfig
    {
        /// <summary>
        /// The user in question
        /// </summary>
        public string MiningUser { get; set; }

        /// <summary>
        /// Is the user valid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// What type of mining is the user doing
        /// </summary>
        public string MiningType { get; set; }

        /// <summary>
        /// User balance
        /// </summary>
        public MiningBalance[] TheBalances { get; set; }

        /// <summary>
        /// Rank in the app
        /// </summary>
        public string UserRank { get; set; }

        /// <summary>
        /// Pools the user can mine from
        /// </summary>
        public MiningPool[] MiningPools { get; set; }

        /// <summary>
        /// Used 
        /// </summary>
        public string MiningWallet { get; set; }

        /// <summary>
        /// Send a message to the user
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Was there an error processing login
        /// </summary>
        public bool IsUserMessageError { get; set; }

    }
}
