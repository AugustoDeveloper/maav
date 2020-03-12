using System.Security.Cryptography;
using System.Text;

namespace MAAV.DataContracts.Extensions
{
    static public class LabelDescrptionExtension
    {
        static public string FormatLabel(this LabelDescription label)
        {
            switch (label)
            {
                case LabelDescription.Alpha : return "alpha";
                case LabelDescription.Beta : return "beta";
                case LabelDescription.PreAlpha : return "pre-alpha";
                case LabelDescription.PreRelease : return "pre-release";
                default: return "";
            }
        }
    }
}