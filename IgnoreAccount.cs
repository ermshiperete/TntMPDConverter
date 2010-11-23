namespace TntMPDConverter
{

	internal class IgnoreAccount : ProcessingDonations
    {
        public IgnoreAccount(Scanner reader) : base(reader)
        {
        }

        public override State NextState()
        {
            string text1 = Reader.ReadLine();
            while (true)
            {
                State state1 = CheckForStartOfNewState(text1);
                if (state1 != null)
                {
                    return state1;
                }
                text1 = Reader.ReadLine();
            }
        }

        public override Donation NextDonation
        {
            get
            {
                IsValid = false;
                return null;
            }
        }

    }
}

