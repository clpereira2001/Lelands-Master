using System;

namespace Vauction.Models
{
  [Serializable]
  partial class ConsignmentContract : IConsignmentContract
  {
    public void UpdateFields(ConsignmentContract cc)
    {
      UpdateFields(cc.ConsignmentID, cc.StatusID, cc.ContractText, cc.FileName);
    }
    public void UpdateFields(long consignmentID, int statusID, string contractText, string fileName)
    {
      ConsignmentID = consignmentID;
      StatusID = statusID;
      ContractText = contractText;
      FileName = fileName;
    }
  }
}
