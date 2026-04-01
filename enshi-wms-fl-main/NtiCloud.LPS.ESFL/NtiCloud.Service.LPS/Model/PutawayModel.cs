using System;
using System.Collections.Generic;
using System.Text;

namespace NtiCloud.Service.WMS.Model
{
   public class PutawayHeaderModel
    {
        public string t_id { get; set; }
        public string client_id { get; set; }
        public string org_id { get; set; }
        public string warehouse_id { get; set; }
        public string doc_no { get; set; }
        public string doc_type { get; set; }
        public string service_type { get; set; }
        public string ref_doc_no { get; set; }
        public string plan_no { get; set; }
        public string bom_code { get; set; }
        public string doc_status { get; set; }
        public string source { get; set; }

        public DateTime? start_date { get; set; }
        public DateTime? finish_date { get; set; }
        public string supplier_id { get; set; }
        public string manufacturer_id { get; set; }
        public string owner_id { get; set; }

        public string src_factory { get; set; }

        public string src_inventory { get; set; }
        public string src_location { get; set; }

        public string dest_factory { get; set; }
        public string dest_inventory { get; set; }
        public string dest_location { get; set; }
        public string shipper { get; set; }
        public string zone_id { get; set; }
        public string dock_id { get; set; }

        public string license_plate { get; set; }

        public string deliver { get; set; }
        public string deliver_tel { get; set; }

        public string trans_permit { get; set; }
        public string contract { get; set; }
        public decimal? net_weight { get; set; }
        public decimal? gross_weight { get; set; }

        public decimal? truck_weight { get; set; }
        public string uom_weight_id { get; set; }
        public string src_addr { get; set; }
        public string dest_addr { get; set; }
        public int? left_task_count { get; set; }

        public string is_auto_created { get; set; }
        public string memo { get; set; }
        public DateTime? create_date { get; set; }
        public string created_by { get; set; }
        public string modify_ip { get; set; }
        public DateTime? modify_date { get; set; }
        public string last_modified_by { get; set; }
        public string act_status { get; set; }
    }

    public class PutawayDetailModel { 
        public string t_id { get; set; }
        public string client_id { get; set; }
        public string org_id { get; set; }
        public string warehouse_id { get; set; }
        public string inbound_id { get; set; }
        public string line_no { get; set; }
        public string item_id { get; set; }
        public string item_code { get; set; }
        public string item_name { get; set; }
        public string lot_no { get; set; }
        public int? given_year { get; set; }
        public string spec { get; set; }
        public DateTime? production_date { get; set; }
        public string exist_barcode { get; set; }
        public string is_full { get; set; }
        public string need_qc { get; set; }
        public string qc_status { get; set; }
        public decimal? plan_qty { get; set; }
        public decimal? processed_qty { get; set; }
        public decimal? finished_qty { get; set; }
        public string uom_qty_id { get; set; }
        public decimal? plan_stat_qty { get; set; }
        public decimal? processed_stat_qty { get; set; }
        public decimal? finished_stat_qty { get; set; }
        public string uom_stat_id { get; set; }
        public string packing_type { get; set; }
        public string supplier_id { get; set; }
        public string src_factory { get; set; }
        public string src_warehouse { get; set; }
        public string src_location { get; set; }
        public string dest_factory { get; set; }

        public string dest_warehouse { get; set; }
        public string dest_location { get; set; }
        public string memo { get; set; }
        public DateTime? create_date { get; set; }
        public string created_by { get; set; }
        public string modify_ip { get; set; }
        public DateTime? modify_date { get; set; }
        public string last_modified_by { get; set; }
        public string act_status { get; set; }

        public string production_date_str { get; set; }
    }

}
