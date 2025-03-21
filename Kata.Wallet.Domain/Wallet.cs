﻿using System.ComponentModel.DataAnnotations;

namespace Kata.Wallet.Domain;

public class Wallet
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public string UserDocument { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public Currency Currency { get; set; }
    public List<Transaction>? IncomingTransactions { get; set; }
    public List<Transaction>? OutgoingTransactions { get; set; }
}

public enum Currency
{
    USD,
    EUR,
    ARS
}
