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
    
    public partial class UserAccounts
    {
        public int ID_UserAccounts { get; set; }
        public int Roles_ID { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
    
        public virtual Roles Roles { get; set; }
    }
}
