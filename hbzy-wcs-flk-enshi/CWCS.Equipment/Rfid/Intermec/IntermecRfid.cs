using Intermec.DataCollection.RFID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CWCS.Equipment.Rfid.Intermec
{
    public class IntermecRfid : IntermecRfidBase, IIntermecRfid
    {
        private BRIReader Reader { get; set; }
        private Form RefForm { get; set; } = new Form();
        public IntermecRfid(string ip, string port = "5005") : base(ip, port)
        {
           
        }

        public override bool Close()
        {
            try
            {
                Reader.StopReadingTags();
                Reader.Dispose();
            }
            catch
            {

            }
            return true;
        }


        public override bool Connect()
        {
            try
            {
                Ping ping = new Ping();
                if (ping.Send(Ip).Status != IPStatus.Success)
                {
                    IsConnected = false;
                    return IsConnected;
                }
                Reader = new BRIReader(RefForm, Ip);
                Reader.Attributes.RFIDTagType = BRIReader.RFIDTagTypes.EPCC1G2;
                Reader.Attributes.ReadTries = 3;
                IsConnected = true;
            }
            catch (Exception)
            {
                Reader = null;
                IsConnected = false;
            }
            return IsConnected;
        }

        public override bool WriteBytes(byte[] bytes)
        {
            return true;
        }

     

        public override void ReadAll()
        {
            try
            {
                ResetData();
                if (Reader != null)
                {
                    Reader.Read();
                    AnalysisTags(Reader.Tags);
                }

            }
            catch (Exception)
            {

            }
        }

        public void AnalysisTags(Tag[] tags)
        {
            if (tags == null)
            {
                return;
            }
            if (tags.Length > 0)
            {
                TID = tags[0].ToString() + tags[0].TagKey;
                if (tags[0].TagFields.ItemCount > 0)
                {
                    TagField[] tagFieldArray = tags[0].TagFields.FieldArray;
                    foreach (TagField field in tagFieldArray)
                    {
                        if (field.Status == TagField.STATUS_OK)
                        {
                            EpcData += field.DataString;
                        }
                        else
                        {
                            //读取失败
                        }
                    }
                }
            }

        }

    }
}
