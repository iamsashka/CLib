//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class Shipments
    {
        public int ID_Shipments { get; set; }
        public int Supplier_ID { get; set; }
        public int Product_ID { get; set; }
        public int Quantity { get; set; }
        public System.DateTime ShipmentDate { get; set; }
    
        public virtual Products Products { get; set; }
        public virtual Suppliers Suppliers { get; set; }
    }
}
