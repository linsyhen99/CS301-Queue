using CsvHelper.Configuration.Attributes;
using RabbitMqQueue.Domain.Enums;

namespace RabbitMqQueue.Domain.Models;

public class Transaction
{
    [Name("id")]
    public string Id { get; set; }

    [Name("card_id")]
    public string CardId { get; set; }

    [Name("merchant")]
    public string Merchant { get; set; }

    [Name("mcc")]
    public int? MCC { get; set; }

    [Name("currency")]
    public CurrencyEnum Currency { get; set; }

    [Name("amount")]
    public decimal Amount { get; set; }

    [Name("sgd_amount")]
    public decimal SgdAmount { get; set; }

    [Name("transaction_id")]
    public string TransactionId { get; set; }

    [Name("transaction_date")]
    public DateTime TransactionDate { get; set; }

    [Name("card_pan")]
    public string CardPan { get; set; }
    
    [Name("card_type")]
    public string CardType { get; set; }

}