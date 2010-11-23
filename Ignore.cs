namespace TntMPDConverter
{

	internal class Ignore : State
    {
        public Ignore(Scanner reader) : base(reader)
        {
            IsValid = false;
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

    }
}

