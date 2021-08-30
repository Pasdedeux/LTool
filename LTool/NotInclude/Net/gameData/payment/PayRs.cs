namespace common.gameData.payment
{
    public class PayRs
    {
        private bool isSuccess = false;
        public bool IsSuccess { get { return isSuccess; } }
        private string recipt;
        public string Recipt { get { return recipt; } }
        public PayRs(bool isSuccess)
        {
            this.isSuccess = isSuccess;
        }
        public PayRs(bool isSuccess,string recipt)
        {
            this.isSuccess = isSuccess;
            this.recipt = recipt;
        }
        
        override public string ToString()
        {
            return "isSuccess:" + isSuccess + ",recipt:" + recipt; 
        }
    }
}