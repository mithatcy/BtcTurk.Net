﻿using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using BtcTurk.Net.Interfaces;
using BtcTurk.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using BtcTurk.Net.Converters;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Threading;

namespace BtcTurk.Net
{
    public class BtcTurkClient : RestClient, IBtcTurkClient
    {
        #region fields
        protected static BtcTurkClientOptions defaultOptions = new BtcTurkClientOptions();
        protected static BtcTurkClientOptions DefaultOptions => defaultOptions.Copy();

        // Api Version
        protected const string V1 = "1";
        protected const string V2 = "2";
        protected const string NoVersion = "";
        protected const string PublicVersion = "2";
        protected const string SignedVersion = "2";

        // Methods
        protected const string GetMethod = "GET";
        protected const string PostMethod = "POST";
        protected const string DeleteMethod = "DELETE";
        protected const string PutMethod = "PUT";

        // V1 Endpoints
        protected const string V1_Balances_Endpoint = "users/balances";
        protected const string V1_Order_Endpoint = "order";
        protected const string V1_AllOrders_Endpoint = "allOrders";
        protected const string V1_OpenOrders_Endpoint = "openOrders";
        protected const string V1_Trades_Endpoint = "trades";
        protected const string V1_TransactionsTrade_Endpoint = "users/transactions/trade";
        protected const string V1_TransactionsTradeByPair_Endpoint = "users/transactions/trade/{pairSymbol}";
        protected const string V1_TransactionsCrypto_Endpoint = "users/transactions/crypto";
        protected const string V1_TransactionsFiat_Endpoint = "users/transactions/fiat";

        // Account
        protected const string Account_OtpQrCode_Endpoint = "account/otp-qr-code";
        protected const string Account_OtpGetHash_Endpoint = "account/otp-get-hash";
        protected const string Account_OtpSms_Endpoint = "account/otp-sms";
        protected const string Account_UsersInfo_Endpoint = "users/info";
        protected const string Account_Info_Endpoint = "account/info";
        protected const string Account_ChangePassword_Endpoint = "account/change-password";
        protected const string Account_UsersLog_Endpoint = "users/logs";
        protected const string Account_AccountLogs_Endpoint = "account/logs";
        protected const string Account_AccountProfile_Endpoint = "account/profile";
        protected const string Account_UsersCommissions_Endpoint = "users/commissions";
        protected const string Account_Commissions_Endpoint = "account/commissions";
        protected const string Account_UsersLimits_Endpoint = "users/limits";
        protected const string Account_Limits_Endpoint = "account/limits";
        protected const string Account_SmsCode_Endpoint = "account/sms-code";
        protected const string Account_Notification_Endpoint = "account/notification";
        protected const string Account_UsersBalances_Endpoint = "users/balances";
        protected const string Account_Balances_Endpoint = "account/balances";
        protected const string Account_Timeout_Endpoint = "account/timeout";
        protected const string Account_History_Endpoint = "account/history";
        protected const string Account_AdvancedOrderHistory_Endpoint = "account/advanced-order-history";

        // AccountAuth
        protected const string AccountAuth_Login_Endpoint = "auth/login";
        protected const string AccountAuth_Otp_Endpoint = "auth/otp";
        protected const string AccountAuth_OtpQrCode_Endpoint = "auth/otp-qr-code";
        protected const string AccountAuth_ResetPassword_Endpoint = "auth/reset-password";

        // AccountBank
        protected const string AccountBank_AccountBanks_Endpoint = "account/banks";
        protected const string AccountBank_AccountBanksId_Endpoint = "account/banks/{id}";

        // AccountTransaction
        protected const string AccountTransaction_UsersTransactionsTrade_Endpoint = "users/transactions/trade";
        protected const string AccountTransaction_UsersTransactionsTradePairSymbol_Endpoint = "users/transactions/trade/{pairSymbol}";
        protected const string AccountTransaction_AccountTransactionsTrade_Endpoint = "account/transactions/trade";
        protected const string AccountTransaction_AccountTransactionsTradePairSymbol_Endpoint = "account/transactions/trade/{pairSymbol}";
        protected const string AccountTransaction_UsersTransactionsCrypto_Endpoint = "users/transactions/crypto";
        protected const string AccountTransaction_AccountTransactionsCrypto_Endpoint = "account/transactions/crypto";
        protected const string AccountTransaction_UsersTransactionsFiat_Endpoint = "users/transactions/fiat";
        protected const string AccountTransaction_AccountTransactionsFiat_Endpoint = "account/transactions/fiat";
        protected const string AccountTransaction_AccountTransactionsOrderHistory_Endpoint = "account/transactions/orderHistory";
        protected const string AccountTransaction_AccountTransactionsOrderTrades_Endpoint = "account/transactions/orderTrades";

        // Alarm
        protected const string Alarm_Endpoint = "alarm";
        protected const string Alarm_IdCurrency_Endpoint = "alarm/{id}/{currency}";

        // ApiAccess
        protected const string ApiAccess_Endpoint = "api/access";
        protected const string ApiAccess_Id_Endpoint = "api/access/{id}";

        // Bank
        protected const string Banks_Endpoint = "banks";
        protected const string Banks_Id_Endpoint = "banks/{id}";

        // CryptoDeposit
        protected const string CryptoDeposit_CurrencySymbol_Endpoint = "deposits/crypto/{currencySymbol}";
        protected const string CryptoDeposit_PendingsCurrencySymbol_Endpoint = "deposits/crypto/pendings/{currencySymbol}";

        // CryptoWithdraw
        protected const string Withdrawals_CryptoPendingCurrencySymbol_Endpoint = "withdrawals/crypto/pending/{currencySymbol}";
        protected const string Withdrawals_Crypto_Endpoint = "withdrawals/crypto";
        protected const string Withdrawals_CryptoId_Endpoint = "withdrawals/crypto/{id}";
        protected const string Withdrawals_CryptoCurrencySymbol_Endpoint = "withdrawals/crypto/{currencySymbol}";
        protected const string Withdrawals_CryptValidationCode_Endpoint = "withdrawals/crypto/validation/{code}";
        protected const string Withdrawals_CryptoReceiverAddressesCurrencySymbol_Endpoint = "withdrawals/crypto/receiver-addresses/{currencySymbol}";
        protected const string Withdrawals_CryptoReceiverAddresses_Endpoint = "withdrawals/crypto/receiver-addresses";
        protected const string Withdrawals_CryptoAddressValidation_Endpoint = "withdrawals/crypto/address-validation";
        protected const string Withdrawals_CryptoReceiverAddressesId_Endpoint = "withdrawals/crypto/receiver-addresses/{id}";

        // Device
        protected const string Device_Set_Endpoint = "device/set";
        protected const string Device_Update_Endpoint = "device/update";

        // Exchange
        protected const string Exchange_Commissions_Endpoint = "exchange/commissions";

        // FiatDeposit
        protected const string FiatDeposit_ServiceType_Endpoint = "deposits/fiat/{serviceType}";
        protected const string FiatDeposit_Bank_Endpoint = "deposits/fiat/bank";
        protected const string FiatDeposit_BankId_Endpoint = "deposits/fiat/bank/{id}";
        protected const string FiatDeposit_SendPapara_Endpoint = "deposits/fiat/send-papara";
        protected const string FiatDeposit_Papara_Endpoint = "deposits/fiat/papara";
        protected const string FiatDeposit_PaparaValidationReference_Endpoint = "deposits/fiat/papara/validation/{reference}";

        // FiatWithdrawal
        protected const string FiatWithdrawal_Bank_Endpoint = "withdrawals/fiat/bank";
        protected const string FiatWithdrawal_BankId_Endpoint = "withdrawals/fiat/bank/{id}";
        protected const string FiatWithdrawal_Papara_Endpoint = "withdrawals/fiat/papara";
        protected const string FiatWithdrawal_ConfirmId_Endpoint = "withdrawals/fiat/confirm/{id}";

        // Home
        protected const string Home_ResourcesLanguage_Endpoint = "resources/{language}.json";

        // KnowYourCustomer
        protected const string KnowYourCustomer_Endpoint = "account/kyc";
        protected const string KnowYourCustomer_FileId_Endpoint = "account/kyc/{fileId}";

        // Korder
        protected const string Korder_Endpoint = "korder";

        // OHLC
        protected const string OhlcEndpoint = "ohlc";
        protected const string OhlcVolumesInBtcEndpoint = "ohlc/volumes/btc";

        // Order
        protected const string Order_Endpoint = "order";
        protected const string Order_Id_Endpoint = "order/{id}";
        protected const string Order_OpenOrders_Endpoint = "openOrders";
        protected const string Order_AllOrders_Endpoint = "allOrders";

        // Order Book
        protected const string OrderBook_Endpoint = "orderBook";

        // PinCode
        protected const string PinCode_Endpoint = "account/pincode";
        protected const string PinCode_Status_Endpoint = "account/pincode-status";

        // Price Graph
        protected const string PriceGraph_Config_Endpoint = "pricegraph/config";
        protected const string PriceGraph_SymbolInfo_Endpoint = "pricegraph/symbol_info";
        protected const string PriceGraph_Symbols_Endpoint = "pricegraph/symbols";
        protected const string PriceGraph_Time_Endpoint = "pricegraph/time";
        protected const string PriceGraph_Search_Endpoint = "pricegraph/search";
        protected const string PriceGraph_History_Endpoint = "pricegraph/history";
        protected const string PriceGraph_MobileHistory_Endpoint = "pricegraph/mobile-history";

        // Server
        protected const string Server_Version_Endpoint = "server/version";
        protected const string Server_Time_Endpoint = "server/time";
        protected const string Server_MobileVersion_Endpoint = "server/mobile/version";
        protected const string Server_ExchangeInfo_Endpoint = "server/exchangeInfo";
        protected const string Server_Ping_Endpoint = "server/ping";

        // Ticker
        protected const string Ticker_Endpoint = "ticker";
        protected const string Ticker_Currency_Endpoint = "ticker/currency";

        // Token
        protected const string Token_Endpoint = "token";
        protected const string Token_Mobile_Endpoint = "token/mobile";
        protected const string Token_Refresh_Endpoint = "token/refresh";

        // Trade
        protected const string TradesEndpoint = "trades";

        #endregion

        #region constructor/destructor
        /// <summary>
        /// Create a new instance of BtcTurkClient using the default options
        /// </summary>
        public BtcTurkClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of the BtcTurkClient with the provided options
        /// </summary>
        public BtcTurkClient(BtcTurkClientOptions options) : base("BtcTurk", options, options.ApiCredentials == null ? null : new BtcTurkAuthenticationProvider(options.ApiCredentials, ArrayParametersSerialization.MultipleValues))
        {
            arraySerialization = ArrayParametersSerialization.MultipleValues;
            Configure(options);
        }
        #endregion

        #region General Methods
        /// <summary>
        /// Sets the default options to use for new clients
        /// </summary>
        /// <param name="options">The options to use for new clients</param>
        public static void SetDefaultOptions(BtcTurkClientOptions options)
        {
            defaultOptions = options;
        }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetAuthenticationProvider(new BtcTurkAuthenticationProvider(new ApiCredentials(apiKey, apiSecret), ArrayParametersSerialization.MultipleValues ));
        }
        #endregion

        #region V1
        public WebCallResult<BtcTurkBalance[]> GetBalances(CancellationToken ct = default) => GetBalancesAsync(ct).Result;
        public async Task<WebCallResult<BtcTurkBalance[]>> GetBalancesAsync(CancellationToken ct = default)
        {
            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkBalance[]>>(GetUrl(V1_Balances_Endpoint, V1), method: HttpMethod.Get, cancellationToken:ct,   checkResult: false, signed: true).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkBalance[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkBalance[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkPlacedOrder> PlaceOrder(string pairSymbol, decimal quantity, BtcTurkOrderSide orderSide, BtcTurkOrderMethod orderMethod, decimal? price = null, decimal? stopPrice = null, string clientOrderId = null, CancellationToken ct = default) => PlaceOrderAsync(pairSymbol, quantity, orderSide, orderMethod, price, stopPrice, clientOrderId,ct).Result;
        public async Task<WebCallResult<BtcTurkPlacedOrder>> PlaceOrderAsync(string pairSymbol,decimal quantity,BtcTurkOrderSide orderSide,BtcTurkOrderMethod orderMethod,decimal? price = null,decimal? stopPrice = null,string clientOrderId = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "pairSymbol", pairSymbol },
                { "quantity", quantity.ToString(CultureInfo.InvariantCulture) },
                { "orderType", JsonConvert.SerializeObject(orderSide, new OrderSideConverter(false)) },
                { "orderMethod", JsonConvert.SerializeObject(orderMethod, new OrderMethodConverter(false)) },
            };
            parameters.AddOptionalParameter("price", price?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("stopPrice", stopPrice?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("newClientOrderId", clientOrderId);

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkPlacedOrder>>(GetUrl(V1_Order_Endpoint, V1), method:HttpMethod.Post, cancellationToken: ct, parameters: parameters, checkResult: false, signed: true).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkPlacedOrder>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkPlacedOrder>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<bool> CancelOrder(long orderId, CancellationToken ct = default) => CancelOrderAsync(orderId,ct).Result;
        public async Task<WebCallResult<bool>> CancelOrderAsync(long orderId, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "id", orderId.ToString(CultureInfo.InvariantCulture) },
            };

            var result = await SendRequestAsync<BtcTurkApiResponse<object>>(GetUrl(V1_Order_Endpoint, V1), method: HttpMethod.Delete, cancellationToken: ct, parameters: parameters, checkResult: false, signed: true).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<bool>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<bool>(result.ResponseStatusCode, result.ResponseHeaders, result.Success, null);
        }

        public WebCallResult<BtcTurkOrder[]> GetAllOrders(string pairSymbol, long? startOrderId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = 100, int? page = 1, CancellationToken ct = default) => GetAllOrdersAsync(pairSymbol, startOrderId, startTime, endTime, limit, page,ct).Result;
        public async Task<WebCallResult<BtcTurkOrder[]>> GetAllOrdersAsync(string pairSymbol, long? startOrderId = null, DateTime? startTime = null, DateTime? endTime = null, int? limit = 100, int? page = 1, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "pairSymbol", pairSymbol },
            };
            parameters.AddOptionalParameter("orderId", startOrderId != null ? startOrderId : null);
            parameters.AddOptionalParameter("startTime", startTime != null ? ToUnixTimestamp(startTime.Value).ToString() : null);
            parameters.AddOptionalParameter("endTime", endTime != null ? ToUnixTimestamp(endTime.Value).ToString() : null);
            parameters.AddOptionalParameter("limit", limit != null ? limit : null);
            parameters.AddOptionalParameter("page", page != null ? page-1 : null);

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkOrder[]>>(GetUrl(V1_AllOrders_Endpoint, V1), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false, signed: true).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkOrder[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkOrder[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkOpenOrders> GetOpenOrders(string pairSymbol = null, CancellationToken ct = default) => GetOpenOrdersAsync(pairSymbol,ct).Result;
        public async Task<WebCallResult<BtcTurkOpenOrders>> GetOpenOrdersAsync(string pairSymbol=null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("pairSymbol", pairSymbol != null ? pairSymbol : null);

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkOpenOrders>>(GetUrl(V1_OpenOrders_Endpoint, V1), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false, signed: true).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkOpenOrders>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkOpenOrders>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkTrade[]> GetTradesV1(string pairSymbol, int last=50, CancellationToken ct = default) => GetTradesV1Async(pairSymbol, last,ct).Result;
        public async Task<WebCallResult<BtcTurkTrade[]>> GetTradesV1Async(string pairSymbol, int last = 50, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "pairSymbol", pairSymbol },
                { "last", last.ToString() },
            };

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkTrade[]>>(GetUrl(V1_Trades_Endpoint, V1), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkTrade[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkTrade[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkTransaction[]> GetTradeTransactions(string[] symbol = null,BtcTurkOrderSide[] type = null,DateTime? startTime = null,DateTime? endTime = null, CancellationToken ct = default) => GetTradeTransactionsAsync(symbol ,type,startTime,endTime ,ct).Result;
        public async Task<WebCallResult<BtcTurkTransaction[]>> GetTradeTransactionsAsync(string[] symbol = null, BtcTurkOrderSide[] type = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("type", JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(type, new OrderSideConverter(true))));
            parameters.AddOptionalParameter("startTime", startTime != null ? ToUnixTimestamp(startTime.Value).ToString() : null);
            parameters.AddOptionalParameter("endTime", endTime != null ? ToUnixTimestamp(endTime.Value).ToString() : null);

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkTransaction[]>>(GetUrl(V1_TransactionsTrade_Endpoint, V1), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkTransaction[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkTransaction[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkPairTransaction[]> GetTradeTransactionsByPair(string pairSymbol, CancellationToken ct = default) => GetTradeTransactionsByPairAsync(pairSymbol,ct).Result;
        public async Task<WebCallResult<BtcTurkPairTransaction[]>> GetTradeTransactionsByPairAsync(string pairSymbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "pairSymbol", pairSymbol },
            };

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkPairTransaction[]>>(GetUrl(V1_TransactionsTradeByPair_Endpoint.Replace("{pairSymbol}",pairSymbol), V1), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkPairTransaction[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkPairTransaction[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkCryptoFiatTransaction[]> GetCryptoTransactions(string[] symbol = null, BtcTurkOrderSide[] type = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default) => GetCryptoTransactionsAsync(symbol, type, startTime, endTime,ct).Result;
        public async Task<WebCallResult<BtcTurkCryptoFiatTransaction[]>> GetCryptoTransactionsAsync(string[] symbol = null, BtcTurkOrderSide[] type = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("type", JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(type, new OrderSideConverter(true))));
            parameters.AddOptionalParameter("startTime", startTime != null ? ToUnixTimestamp(startTime.Value).ToString() : null);
            parameters.AddOptionalParameter("endTime", endTime != null ? ToUnixTimestamp(endTime.Value).ToString() : null);

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkCryptoFiatTransaction[]>>(GetUrl(V1_TransactionsCrypto_Endpoint, V1), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkCryptoFiatTransaction[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkCryptoFiatTransaction[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkCryptoFiatTransaction[]> GetFiatTransactions(string[] symbol = null, BtcTurkOrderSide[] type = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default) => GetFiatTransactionsAsync(symbol, type, startTime, endTime,ct).Result;
        public async Task<WebCallResult<BtcTurkCryptoFiatTransaction[]>> GetFiatTransactionsAsync(string[] symbol = null, BtcTurkOrderSide[] type = null, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("symbol", symbol);
            parameters.AddOptionalParameter("type", JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(type, new OrderSideConverter(true))));
            parameters.AddOptionalParameter("startTime", startTime != null ? ToUnixTimestamp(startTime.Value).ToString() : null);
            parameters.AddOptionalParameter("endTime", endTime != null ? ToUnixTimestamp(endTime.Value).ToString() : null);

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkCryptoFiatTransaction[]>>(GetUrl(V1_TransactionsFiat_Endpoint, V1), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkCryptoFiatTransaction[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkCryptoFiatTransaction[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        #endregion

        #region Exchange
        public WebCallResult<BtcTurkCommission> GetExchangeCommissions( CancellationToken ct = default) => GetExchangeCommissionsAsync(ct).Result;
        public async Task<WebCallResult<BtcTurkCommission>> GetExchangeCommissionsAsync( CancellationToken ct = default)
        {
            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkCommission>>(GetUrl(Exchange_Commissions_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken:ct, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkCommission>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkCommission>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }
        #endregion

        #region Home
        public WebCallResult<BtcTurkResources> GetResources(string language = "tr-TR", CancellationToken ct = default) => GetResourcesAsync(language,ct).Result;
        public async Task<WebCallResult<BtcTurkResources>> GetResourcesAsync(string language = "tr-TR", CancellationToken ct = default)
        {
            var result = await SendRequestAsync<BtcTurkResources>(GetUrl(Home_ResourcesLanguage_Endpoint.Replace("{language}", language), NoVersion), method:HttpMethod.Get, cancellationToken: ct, checkResult:false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkResources>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkResources>(result.ResponseStatusCode, result.ResponseHeaders, result.Data, null);
        }
        #endregion

        #region OHLC
        public WebCallResult<BtcTurkOhlc[]> GetOhlc(string pairSymbol, int last = 50, CancellationToken ct = default) => GetOhlcAsync(pairSymbol, last,ct).Result;
        public async Task<WebCallResult<BtcTurkOhlc[]>> GetOhlcAsync(string pairSymbol, int last = 50, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "pairSymbol", pairSymbol },
            };
            parameters.AddOptionalParameter("last", last);

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkOhlc[]>>(GetUrl(OhlcEndpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkOhlc[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkOhlc[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkOhlcVolumes[]> GetOhlcVolumesInBtc(CancellationToken ct = default) => GetOhlcVolumesInBtcAsync(ct).Result;
        public async Task<WebCallResult<BtcTurkOhlcVolumes[]>> GetOhlcVolumesInBtcAsync(CancellationToken ct = default)
        {
            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkOhlcVolumes[]>>(GetUrl(OhlcVolumesInBtcEndpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkOhlcVolumes[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkOhlcVolumes[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }
        #endregion

        #region Order Book
        public WebCallResult<BtcTurkOrderBook> GetOrderBook(string pairSymbol, int limit = 100, CancellationToken ct = default) => GetOrderBookAsync(pairSymbol, limit,ct).Result;
        public async Task<WebCallResult<BtcTurkOrderBook>> GetOrderBookAsync(string pairSymbol, int limit = 100, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "pairSymbol", pairSymbol },
                { "limit", limit },
            };

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkOrderBook>>(GetUrl(OrderBook_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkOrderBook>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkOrderBook>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }
        #endregion

        #region Price Graph
        public WebCallResult<BtcTurkPriceGraphConfig> GetPriceGraphConfig(CancellationToken ct = default) => GetPriceGraphConfigAsync(ct).Result;
        public async Task<WebCallResult<BtcTurkPriceGraphConfig>> GetPriceGraphConfigAsync(CancellationToken ct = default)
        {
            var result = await SendRequestAsync<BtcTurkPriceGraphConfig>(GetUrl(PriceGraph_Config_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkPriceGraphConfig>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkPriceGraphConfig>(result.ResponseStatusCode, result.ResponseHeaders, result.Data, null);
        }

        public WebCallResult<BtcTurkPriceGraphSymbolInfo> GetPriceGraphSymbolInfo(string group = "", CancellationToken ct = default) => GetPriceGraphSymbolInfoAsync(group,ct).Result;
        public async Task<WebCallResult<BtcTurkPriceGraphSymbolInfo>> GetPriceGraphSymbolInfoAsync(string group = "", CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "group", group },
            };

            var result = await SendRequestAsync<BtcTurkPriceGraphSymbolInfo>(GetUrl(PriceGraph_SymbolInfo_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkPriceGraphSymbolInfo>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkPriceGraphSymbolInfo>(result.ResponseStatusCode, result.ResponseHeaders, result.Data, null);
        }

        public WebCallResult<BtcTurkPriceGraphSymbols> GetPriceGraphSymbols(string symbol, CancellationToken ct = default) => GetPriceGraphSymbolsAsync(symbol,ct).Result;
        public async Task<WebCallResult<BtcTurkPriceGraphSymbols>> GetPriceGraphSymbolsAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
            };

            var result = await SendRequestAsync<BtcTurkPriceGraphSymbols>(GetUrl(PriceGraph_Symbols_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkPriceGraphSymbols>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkPriceGraphSymbols>(result.ResponseStatusCode, result.ResponseHeaders, result.Data, null);
        }

        public WebCallResult<BtcTurkKline[]> GetPriceGraphHistory(string symbol, BtcTurkPeriod period, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default) => GetPriceGraphHistoryAsync(symbol, period, startTime, endTime,ct).Result;
        public async Task<WebCallResult<BtcTurkKline[]>> GetPriceGraphHistoryAsync(string symbol, BtcTurkPeriod period, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        => await GetPriceGraphHistoryCoreAsync(PriceGraph_History_Endpoint, symbol, period, startTime, endTime);

        public WebCallResult<BtcTurkKline[]> GetPriceGraphMobileHistory(string symbol, BtcTurkPeriod period, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default) => GetPriceGraphMobileHistoryAsync(symbol, period, startTime, endTime,ct).Result;
        public async Task<WebCallResult<BtcTurkKline[]>> GetPriceGraphMobileHistoryAsync(string symbol, BtcTurkPeriod period, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        => await GetPriceGraphHistoryCoreAsync(PriceGraph_MobileHistory_Endpoint, symbol, period, startTime, endTime,ct);

        protected virtual async Task<WebCallResult<BtcTurkKline[]>> GetPriceGraphHistoryCoreAsync(string endpoint, string symbol, BtcTurkPeriod period, DateTime? startTime = null, DateTime? endTime = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object> {
                { "symbol", symbol },
                { "resolution", JsonConvert.SerializeObject(period, new PeriodEnumConverter(false)) }
            };
            parameters.AddOptionalParameter("from", startTime != null ? startTime.Value.ToUnixTimeSeconds().ToString() : "1");
            parameters.AddOptionalParameter("to", endTime != null ? endTime.Value.ToUnixTimeSeconds().ToString() : DateTime.UtcNow.ToUnixTimeSeconds().ToString());

            var result = await SendRequestAsync<BtcTurkKlineData>(GetUrl(endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success || result.Data == null
                || result.Data.Times == null
                || result.Data.Opens == null
                || result.Data.Highs == null
                || result.Data.Lows == null
                || result.Data.Closes == null
                || result.Data.Volumes == null)
                return new WebCallResult<BtcTurkKline[]>(result.ResponseStatusCode, result.ResponseHeaders, null, result.Error);

            // Parse Result
            List<BtcTurkKline> response = new List<BtcTurkKline>();
            var maxRows = Math.Min(result.Data.Times.Length, result.Data.Opens.Length);
            maxRows = Math.Min(maxRows, result.Data.Highs.Length);
            maxRows = Math.Min(maxRows, result.Data.Lows.Length);
            maxRows = Math.Min(maxRows, result.Data.Closes.Length);
            maxRows = Math.Min(maxRows, result.Data.Volumes.Length);

            for (int i = 0; i < maxRows; i++)
            {
                response.Add(new BtcTurkKline
                {
                    OpenDateTime = result.Data.Times[i].FromUnixTimeSeconds(),
                    Open = result.Data.Opens[i],
                    High = result.Data.Highs[i],
                    Low = result.Data.Lows[i],
                    Close = result.Data.Closes[i],
                    Volume = result.Data.Volumes[i],
                });
            }

            return new WebCallResult<BtcTurkKline[]>(result.ResponseStatusCode, result.ResponseHeaders, response.ToArray(), null);
        }
        #endregion

        #region Server
        public WebCallResult<BtcTurkServerVersion> GetServerVersion( CancellationToken ct = default) => GetServerVersionAsync(ct).Result;
        public async Task<WebCallResult<BtcTurkServerVersion>> GetServerVersionAsync( CancellationToken ct = default)
        {
            var result = await SendRequestAsync<BtcTurkServerVersion>(GetUrl(Server_Version_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkServerVersion>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkServerVersion>(result.ResponseStatusCode, result.ResponseHeaders, result.Data, null);
        }

        public WebCallResult<BtcTurkTime> GetServerTime( CancellationToken ct = default) => GetServerTimeAsync(ct).Result;
        public async Task<WebCallResult<BtcTurkTime>> GetServerTimeAsync( CancellationToken ct = default)
        {
            var result = await SendRequestAsync<BtcTurkTime>(GetUrl(Server_Time_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkTime>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkTime>(result.ResponseStatusCode, result.ResponseHeaders, result.Data, null);
        }

        public WebCallResult<int> GetServerMobileVersion(string os, string version, CancellationToken ct = default) => GetServerMobileVersionAsync(os, version,ct).Result;
        public async Task<WebCallResult<int>> GetServerMobileVersionAsync(string os, string version, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "os", os },
                { "ver", version },
            };

            var result = await SendRequestAsync<BtcTurkApiResponse<int>>(GetUrl(Server_MobileVersion_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken:ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<int>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<int>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkServerExchangeInfo> GetServerExchangeInfo( CancellationToken ct = default) => GetServerExchangeInfoAsync(ct).Result;
        public async Task<WebCallResult<BtcTurkServerExchangeInfo>> GetServerExchangeInfoAsync( CancellationToken ct = default)
        {
            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkServerExchangeInfo>>(GetUrl(Server_ExchangeInfo_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkServerExchangeInfo>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkServerExchangeInfo>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkServerPing> GetServerPing( CancellationToken ct = default) => GetServerPingAsync(ct).Result;
        public async Task<WebCallResult<BtcTurkServerPing>> GetServerPingAsync( CancellationToken ct = default)
        {
            var result = await SendRequestAsync<BtcTurkServerPing>(GetUrl(Server_Ping_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkServerPing>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkServerPing>(result.ResponseStatusCode, result.ResponseHeaders, result.Data, null);
        }
        #endregion

        #region Ticker
        public WebCallResult<BtcTurkTicker[]> GetTicker(string pairSymbol = "", CancellationToken ct = default) => GetTickerAsync(pairSymbol,ct).Result;
        public async Task<WebCallResult<BtcTurkTicker[]>> GetTickerAsync(string pairSymbol = "", CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "pairSymbol", pairSymbol },
            };

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkTicker[]>>(GetUrl(Ticker_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkTicker[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkTicker[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }

        public WebCallResult<BtcTurkTicker[]> GetTickerByCurrency(string currencySymbol, CancellationToken ct = default) => GetTickerByCurrencyAsync(currencySymbol,ct).Result;
        public async Task<WebCallResult<BtcTurkTicker[]>> GetTickerByCurrencyAsync(string currencySymbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "symbol", currencySymbol },
            };

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkTicker[]>>(GetUrl(Ticker_Currency_Endpoint, PublicVersion), method: HttpMethod.Get, cancellationToken: ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkTicker[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkTicker[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }
        #endregion

        #region Trade
        public WebCallResult<BtcTurkTrade[]> GetTradesV2(string pairSymbol/*, int last=50*/, CancellationToken ct = default) => GetTradesV2Async(pairSymbol/*, last*/,ct).Result;
        public async Task<WebCallResult<BtcTurkTrade[]>> GetTradesV2Async(string pairSymbol/*, int last = 50*/, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "pairSymbol", pairSymbol },
                /*{ "last", last.ToString() },*/
            };

            var result = await SendRequestAsync<BtcTurkApiResponse<BtcTurkTrade[]>>(GetUrl(TradesEndpoint, PublicVersion), method:HttpMethod.Get,cancellationToken:ct, parameters: parameters, checkResult: false).ConfigureAwait(false);
            if (!result.Success) return WebCallResult<BtcTurkTrade[]>.CreateErrorResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);

            return new WebCallResult<BtcTurkTrade[]>(result.ResponseStatusCode, result.ResponseHeaders, result.Data.Data, null);
        }
        #endregion

        #region Private Methods
        protected override IRequest ConstructRequest(Uri uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization, int requestId, Dictionary<string, string> additionalHeaders)
        {
            return this.BtcTurkConstructRequest(uri, method, parameters, signed, parameterPosition, arraySerialization, requestId, additionalHeaders);
        }
        protected virtual IRequest BtcTurkConstructRequest(Uri uri, HttpMethod method, Dictionary<string, object> parameters, bool signed, HttpMethodParameterPosition parameterPosition, ArrayParametersSerialization arraySerialization, int requestId, Dictionary<string, string> additionalHeaders)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();

            var uriString = uri.ToString();
            if (authProvider != null)
                parameters = authProvider.AddAuthenticationToParameters(uriString, method, parameters, signed, parameterPosition, arraySerialization);

            if ((method == HttpMethod.Get || method == HttpMethod.Delete|| parameterPosition == HttpMethodParameterPosition.InUri) && parameters?.Any() == true)
                uriString += "?" + parameters.CreateParamString(true, ArrayParametersSerialization.MultipleValues);

            if (method == HttpMethod.Post && signed)
            {
                var uriParamNames = new[] { "AccessKeyId", "SignatureMethod", "SignatureVersion", "Timestamp", "Signature" };
                var uriParams = parameters.Where(p => uriParamNames.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value);
                uriString += "?" + uriParams.CreateParamString(true, ArrayParametersSerialization.MultipleValues);
                parameters = parameters.Where(p => !uriParamNames.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value);
            }

            var request = RequestFactory.Create(method, uriString, requestId);
            var contentType = requestBodyFormat == RequestBodyFormat.Json ? Constants.JsonContentHeader : Constants.FormContentHeader;
            request.Accept = Constants.JsonContentHeader;
            
            var headers = new Dictionary<string, string>();
            if (authProvider != null)
                headers = authProvider.AddAuthenticationToHeaders(uriString, method, parameters!, signed, parameterPosition, arraySerialization);

            foreach (var header in headers)
                request.AddHeader(header.Key, header.Value);

            if ((method == HttpMethod.Post || method == HttpMethod.Put) && parameterPosition != HttpMethodParameterPosition.InUri)
            {
                if (parameters?.Any() == true)
                    WriteParamBody(request, parameters, contentType);
                else
                    request.SetContent("{}", contentType);
            }

            return request;
        }

        protected override Error ParseErrorResponse(JToken error)
        {
            return this.BtcTurkParseErrorResponse(error);
        }
        protected virtual Error BtcTurkParseErrorResponse(JToken error)
        {
            if (error["code"] == null || error["message"] == null)
                return new ServerError(error.ToString());

            return new ServerError($"{(string)error["code"]}, {(string)error["message"]}");
        }

        protected virtual Uri GetUrl(string endpoint, string version = null)
        {
            return string.IsNullOrEmpty(version) ? new Uri($"{BaseAddress.TrimEnd('/')}/{endpoint}") : new Uri($"{BaseAddress.TrimEnd('/')}/v{version}/{endpoint}");
        }

        protected virtual void Configure(BtcTurkClientOptions options)
        {
        }

        protected static long ToUnixTimestamp(DateTime time)
        {
            return (long)(time - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        #endregion

    }
}
