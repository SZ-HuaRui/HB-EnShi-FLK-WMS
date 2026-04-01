using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NTI.WMS.CallTerminalSystem.WebCallEnd.Common
{
     /// <summary>
    /// GridView 扩展控件
    /// @author:jianyi0115@163.com
    /// </summary>
     public class GridView : System.Web.UI.WebControls.GridView
    {       
        private bool _enableEmptyContentRender = true ;
        /// <summary>
        /// 是否数据为空时显示标题行
        /// </summary>
        public bool EnableEmptyContentRender
        {
            set { _enableEmptyContentRender = value; }
            get { return _enableEmptyContentRender; }
        }

        private string _EmptyDataCellCssClass ;
        /// <summary>
        /// 为空时信息单元格样式类
        /// </summary>
        public string EmptyDataCellCssClass
        {
            set { _EmptyDataCellCssClass = value ; }
            get { return _EmptyDataCellCssClass ; }
        }

        /// <summary>
        /// 为空时输出内容
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void RenderEmptyContent(HtmlTextWriter writer)
        {
            Table t = new Table(); //create a table
            t.CssClass = this.CssClass; //copy all property
            t.GridLines = this.GridLines;
            t.BorderStyle = this.BorderStyle;
            t.BorderWidth = this.BorderWidth;
            t.CellPadding = this.CellPadding;
            t.CellSpacing = this.CellSpacing;

            t.HorizontalAlign = this.HorizontalAlign;

            t.Width = this.Width;

            t.CopyBaseAttributes(this);

            TableRow row = new TableRow();
            t.Rows.Add(row);

            foreach (DataControlField f in this.Columns) //generate table header
            {
                TableCell cell = new TableCell();

                cell.Text = f.HeaderText;

                cell.CssClass = "TdHeaderStyle1"; //这里把表头样式写死了

                row.Cells.Add(cell);
            }

            TableRow row2 = new TableRow();
            t.Rows.Add(row2);

            TableCell msgCell = new TableCell();
            msgCell.CssClass = this._EmptyDataCellCssClass;

            if (this.EmptyDataTemplate != null) //the second row, use the template
            {
                this.EmptyDataTemplate.InstantiateIn(msgCell);
            }
            else //the second row, use the EmptyDataText
            {
                msgCell.Text = this.EmptyDataText;
            }

            msgCell.HorizontalAlign = HorizontalAlign.Center;
            msgCell.ColumnSpan = this.Columns.Count;

            row2.Cells.Add(msgCell);

            t.RenderControl(writer);
       }

        protected override void  Render(HtmlTextWriter writer)
        {
            if ( _enableEmptyContentRender && ( this.Rows.Count == 0 || this.Rows[0].RowType == DataControlRowType.EmptyDataRow) )
            {
                RenderEmptyContent(writer);
            }
            else
            {
                base.Render(writer);
            }
        }    

    }
}