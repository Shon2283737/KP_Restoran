//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Restoran
{
    using System;
    using System.Collections.Generic;
    
    public partial class TbOrders
    {
        public int id { get; set; }
        public string data { get; set; }
        public int sum { get; set; }
        public int idUser { get; set; }
        public Nullable<int> idKlient { get; set; }
        public Nullable<int> idDish { get; set; }
    
        public virtual TbDishes TbDishes { get; set; }
        public virtual TbKlients TbKlients { get; set; }
        public virtual TbUsers TbUsers { get; set; }
    }
}