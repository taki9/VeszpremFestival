
namespace client
{
    public class Head
    {
        public string STATUS { get; set; }
        public string STATUSCODE { get; set; }
    }

    public class Body
    {
        public string MESSAGE { get; set; }
    }

    public class Message
    {

        public Message(string p, string msg)
        {
            this.head = new Head();
            this.body = new Body();
            this.head.STATUS = p;
            this.body.MESSAGE = msg;
        }

        public Message(string p, string statuscode, string msg)
        {
            this.head = new Head();
            this.body = new Body();
            this.head.STATUS = p;
            this.head.STATUSCODE = statuscode;
            this.body.MESSAGE = msg;
        }


        public Message()
        {
            this.head = null;
            this.body = null;
        }
        public Head head { get; set; }
        public Body body { get; set; }
    }
}
