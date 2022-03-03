using CsvHelper.Configuration;
using RabbitMqQueue.Domain.Models;

namespace RabbitMqQueue.Domain.Mappings;

public class TransactionMap : ClassMap<Transaction>
{
    public TransactionMap()
    {
        Map(tx => tx.Id).Name("id");
        Map(tx => tx.CardId).Name("card_id");
        Map(tx => tx.Merchant).Name("merchant");
        Map(tx => tx.MCC).Name("mcc");
        Map(tx => tx.Currency).Name("currency");
        Map(tx => tx.Amount).Name("amount");
        Map(tx => tx.SgdAmount).Name("sgd_amount");
        Map(tx => tx.TransactionId).Name("transaction_id");
        Map(tx => tx.TransactionDate).Name("transaction_date");
        Map(tx => tx.CardPan).Name("card_pan");
        Map(tx => tx.CardType).Name("card_type");
    }
}