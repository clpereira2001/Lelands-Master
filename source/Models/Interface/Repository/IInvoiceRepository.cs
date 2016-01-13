using System.Collections.Generic;

namespace Vauction.Models
{
  public interface IInvoiceRepository
  {
    List<PriceRealized> GetPriceRealizedForEvent(long event_id);
    List<LinkParams> GetUserInvoicesForPage(long user_id);
    UserInvoice GetUserInvoice(long userinvoice_id);
    decimal GetUserInvoicePaidAmount(long userinvoice_id);
    List<InvoiceDetail> GetInvoiceDetailsByUserInvoice(long userinvoice_id);
    List<LinkParams> GetConsignorStatementsForPage(long user_id);
    List<ConsignmentDetail> GetConsignmentDetailsByConsignmentID(long consignmentID);
    Consignment GetConsignment(long consignment_id);
    ConsignmentContract GetConsignmentContract(long consignmentID);
    ConsignmentContract UpdateConsignmentContract(ConsignmentContract cc);
    Specialist GetSpecialist(long specialistID);
    UICInvoice GetConsignmentTotals(long consignment_id);
  }
}