using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectConnectionPredictControl
{
    public class FaultModel
    {
        private string faultName, faultType, faultPosition;
 
      
        public string FaultName
        {
            get
            {
                return faultName;
            }
            set
            {
                faultName = value;
            }
        }
        public string FaultType
        {
            get
            {
                return faultType;
            }
            set
            {
                faultType = value;
            }
        }
        public string FaultPosition
        {
            get
            {
                return faultPosition;
            }
            set
            {
                faultPosition = value;
            }
        }
    }

}