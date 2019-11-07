//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

namespace StoreSales
{
    using System.Runtime.Serialization;

    [DataContract(Name = "Order")]
    public class Order
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "product")]
        public string Product { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        [DataMember(Name = "customer")]
        public string Customer { get; set; }
    }
}
