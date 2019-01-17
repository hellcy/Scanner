using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using Scanner.Models;

namespace Scanner
{
    public class WebGlobal
    {
        public static string ConnStr { get; set; }
        public static Guid guid { get; set; }
        public static DataTable GramLine { get; set; }
        public static DataTable ColorLine { get; set; }
        public static DataTable RailsPosts { get; set; }
        public static DataTable SquarePosts50 { get; set; }
        public static DataTable SquarePosts60 { get; set; }
        public static DataTable SquarePosts65 { get; set; }
        public static DataTable Lattice { get; set; }
        public static DataTable Plinths { get; set; }
        public static DataTable SmartSlatAng { get; set; }

        //public static Fence fence { get; set; }
        //public static NewOrder newOrder { get; set; }

        public static DataTable dt_OrderDetails { get; set; }

        public static string CurrForm { get; set; }
        public static double TotalWeight { get; set; }

        public static string Company { get; set; }
        public static string OrderBy { get; set; }
        public static string Mobile { get; set; }
        public static string Email { get; set; }
        public static bool RequestForDelivery { get; set; }
        public static string OrderNo { get; set; }
        public static string Reference { get; set; }
        public static string ACCNO { get; set; }
        public static string BranchIDDealWith { get; set; }
        public static bool finalPage { get; set; }

        //public static void ClearAllOrders()
        //{           
        //    TotalWeight = 0;
        //    GramLine = null;
        //    ColorLine = null;
        //    RailsPosts = null;
        //    SquarePosts50 = null;
        //    SquarePosts60 = null;
        //    SquarePosts65 = null;
        //    Lattice = null;
        //    Plinths = null;
        //    SmartSlatAng = null;

        //    dt_OrderDetails = null;
        //    fence = null;
        //    ACCNO = "";
        //    Company = "";
        //    OrderBy = "";
        //    Mobile = "";
        //    Email = "";
        //    RequestForDelivery = false;
        //    OrderNo = "";
        //    Reference = "";
        //    ACCNO = "";
        //}

        //private static void ClearAllTmpTables()
        //{
        //    if (GramLine != null)
        //    {
        //        for (int i = 0; i < GramLine.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < GramLine.Columns.Count; j++)
        //            {
        //                GramLine.Rows[i][j] = "";
        //            }
        //        }
        //    }

        //    if (ColorLine != null)
        //    {
        //        for (int i = 0; i < ColorLine.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < ColorLine.Columns.Count; j++)
        //            {
        //                ColorLine.Rows[i][j] = "";
        //            }
        //        }
        //    }

        //    if (RailsPosts != null)
        //    {
        //        for (int i = 0; i < ColorLine.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < ColorLine.Columns.Count; j++)
        //            {
        //                ColorLine.Rows[i][j] = "";
        //            }
        //        }
        //    }

        //    if (SquarePosts50 != null)
        //    {
        //        for (int i = 0; i < SquarePosts50.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SquarePosts50.Columns.Count; j++)
        //            {
        //                SquarePosts50.Rows[i][j] = "";
        //            }
        //        }
        //    }

        //    if (SquarePosts60 != null)
        //    {
        //        for (int i = 0; i < SquarePosts60.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SquarePosts60.Columns.Count; j++)
        //            {
        //                SquarePosts60.Rows[i][j] = "";
        //            }
        //        }
        //    }

        //    if (SquarePosts65 != null)
        //    {
        //        for (int i = 0; i < SquarePosts65.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SquarePosts65.Columns.Count; j++)
        //            {
        //                SquarePosts65.Rows[i][j] = "";
        //            }
        //        }
        //    }

        //    if (Lattice != null)
        //    {
        //        for (int i = 0; i < Lattice.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < Lattice.Columns.Count; j++)
        //            {
        //                Lattice.Rows[i][j] = "";
        //            }
        //        }
        //    }

        //    if (SmartSlatAng != null)
        //    {
        //        for (int i = 0; i < SmartSlatAng.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SmartSlatAng.Columns.Count; j++)
        //            {
        //                SmartSlatAng.Rows[i][j] = "";
        //            }
        //        }
        //    }
        //}


        //public static IList<NewOrderDetail> CreateNewOrderDetail()
        //{
        //    IList<NewOrderDetail> nOrderDetails = new List<NewOrderDetail>();

        //    if ((GramLine != null) && (GramLine.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < GramLine.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < GramLine.Columns.Count; j++)
        //            {
        //                if (GramLine.Rows[i][GramLine.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                    nOrderDetail.TYPE = "GramLine";
        //                    nOrderDetail.LENGTH = GramLine.Columns[j].ColumnName;
        //                    nOrderDetail.COLOUR = GramLine.Rows[i]["COLOUR"].ToString();
        //                    nOrderDetail.QTY = GramLine.Rows[i][GramLine.Columns[j].ColumnName].ToString();
        //                    nOrderDetails.Add(nOrderDetail);
        //                }
        //            }
        //        }
        //    }

        //    if ((ColorLine != null) && (ColorLine.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < ColorLine.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < ColorLine.Columns.Count; j++)
        //            {
        //                if (ColorLine.Rows[i][ColorLine.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                    nOrderDetail.TYPE = "ColorLine";
        //                    nOrderDetail.LENGTH = ColorLine.Columns[j].ColumnName;
        //                    nOrderDetail.COLOUR = ColorLine.Rows[i]["COLOUR"].ToString();
        //                    nOrderDetail.QTY = ColorLine.Rows[i][ColorLine.Columns[j].ColumnName].ToString();
        //                    nOrderDetails.Add(nOrderDetail);
        //                }
        //            }
        //        }
        //    }

        //    if ((RailsPosts != null) && (RailsPosts.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < RailsPosts.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < RailsPosts.Columns.Count; j++)
        //            {
        //                if (RailsPosts.Rows[i][RailsPosts.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                    nOrderDetail.TYPE = "RailsPosts";
        //                    nOrderDetail.LENGTH = RailsPosts.Columns[j].ColumnName;
        //                    nOrderDetail.COLOUR = RailsPosts.Rows[i]["COLOUR"].ToString();
        //                    nOrderDetail.QTY = RailsPosts.Rows[i][RailsPosts.Columns[j].ColumnName].ToString();
        //                    nOrderDetails.Add(nOrderDetail);
        //                }
        //            }
        //        }
        //    }


        //    if ((SquarePosts50 != null) && (SquarePosts50.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < SquarePosts50.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SquarePosts50.Columns.Count; j++)
        //            {
        //                if (SquarePosts50.Rows[i][SquarePosts50.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                    nOrderDetail.TYPE = "SquarePosts50";
        //                    nOrderDetail.LENGTH = SquarePosts50.Columns[j].ColumnName;
        //                    nOrderDetail.COLOUR = SquarePosts50.Rows[i]["COLOUR"].ToString();
        //                    nOrderDetail.QTY = SquarePosts50.Rows[i][SquarePosts50.Columns[j].ColumnName].ToString();
        //                    nOrderDetails.Add(nOrderDetail);
        //                }
        //            }
        //        }
        //    }


        //    if ((SquarePosts60 != null) && (SquarePosts60.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < SquarePosts60.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SquarePosts60.Columns.Count; j++)
        //            {
        //                if (SquarePosts60.Rows[i][SquarePosts60.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                    nOrderDetail.TYPE = "SquarePosts60";
        //                    nOrderDetail.LENGTH = SquarePosts60.Columns[j].ColumnName;
        //                    nOrderDetail.COLOUR = SquarePosts60.Rows[i]["COLOUR"].ToString();
        //                    nOrderDetail.QTY = SquarePosts60.Rows[i][SquarePosts60.Columns[j].ColumnName].ToString();
        //                    nOrderDetails.Add(nOrderDetail);
        //                }
        //            }
        //        }
        //    }


        //    if ((SquarePosts65 != null) && (SquarePosts65.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < SquarePosts65.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SquarePosts65.Columns.Count; j++)
        //            {
        //                if (SquarePosts65.Rows[i][SquarePosts65.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                    nOrderDetail.TYPE = "SquarePosts65";
        //                    nOrderDetail.LENGTH = SquarePosts65.Columns[j].ColumnName;
        //                    nOrderDetail.COLOUR = SquarePosts65.Rows[i]["COLOUR"].ToString();
        //                    nOrderDetail.QTY = SquarePosts65.Rows[i][SquarePosts65.Columns[j].ColumnName].ToString();
        //                    nOrderDetails.Add(nOrderDetail);
        //                }
        //            }
        //        }
        //    }


        //    if ((Lattice != null) && (Lattice.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < Lattice.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < Lattice.Columns.Count; j++)
        //            {
        //                if (Lattice.Rows[i][Lattice.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                    nOrderDetail.TYPE = "Lattice";
        //                    nOrderDetail.LENGTH = Lattice.Columns[j].ColumnName;
        //                    nOrderDetail.COLOUR = Lattice.Rows[i]["COLOUR"].ToString();
        //                    nOrderDetail.QTY = Lattice.Rows[i][Lattice.Columns[j].ColumnName].ToString();
        //                    nOrderDetails.Add(nOrderDetail);
        //                }
        //            }
        //        }
        //    }


        //    if ((Plinths != null) && (Plinths.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < Plinths.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < Plinths.Columns.Count; j++)
        //            {
        //                if (Plinths.Rows[i][Plinths.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                    nOrderDetail.TYPE = "Plinths";
        //                    nOrderDetail.LENGTH = Plinths.Columns[j].ColumnName;
        //                    nOrderDetail.COLOUR = Plinths.Rows[i]["COLOUR"].ToString();
        //                    nOrderDetail.QTY = Plinths.Rows[i][Plinths.Columns[j].ColumnName].ToString();
        //                    nOrderDetails.Add(nOrderDetail);
        //                }
        //            }
        //        }
        //    }


        //    if ((SmartSlatAng != null) && (SmartSlatAng.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < SmartSlatAng.Rows.Count; i++)
        //        {
        //            if ((SmartSlatAng.Rows[i][SmartSlatAng.Columns[1].ColumnName].ToString() != "") &&
        //                    (SmartSlatAng.Rows[i][SmartSlatAng.Columns[2].ColumnName].ToString() != ""))
        //            {
        //                NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                nOrderDetail.TYPE = "SmartSlatAng";
        //                nOrderDetail.LENGTH = SmartSlatAng.Rows[i][SmartSlatAng.Columns[2].ColumnName].ToString();
        //                nOrderDetail.COLOUR = SmartSlatAng.Rows[i]["COLOUR"].ToString();
        //                nOrderDetail.QTY = SmartSlatAng.Rows[i][SmartSlatAng.Columns[1].ColumnName].ToString();
        //                nOrderDetails.Add(nOrderDetail);
        //            }


        //            if ((SmartSlatAng.Rows[i][SmartSlatAng.Columns[3].ColumnName].ToString() != "") &&
        //                    (SmartSlatAng.Rows[i][SmartSlatAng.Columns[4].ColumnName].ToString() != ""))
        //            {
        //                NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                nOrderDetail.TYPE = "SmartSlatAng";
        //                nOrderDetail.LENGTH = SmartSlatAng.Rows[i][SmartSlatAng.Columns[4].ColumnName].ToString();
        //                nOrderDetail.COLOUR = SmartSlatAng.Rows[i]["COLOUR"].ToString();
        //                nOrderDetail.QTY = SmartSlatAng.Rows[i][SmartSlatAng.Columns[3].ColumnName].ToString();
        //                nOrderDetails.Add(nOrderDetail);
        //            }


        //            if ((SmartSlatAng.Rows[i][SmartSlatAng.Columns[5].ColumnName].ToString() != "") &&
        //                    (SmartSlatAng.Rows[i][SmartSlatAng.Columns[6].ColumnName].ToString() != ""))
        //            {
        //                NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                nOrderDetail.TYPE = "SmartSlatAng";
        //                nOrderDetail.LENGTH = SmartSlatAng.Rows[i][SmartSlatAng.Columns[6].ColumnName].ToString();
        //                nOrderDetail.COLOUR = SmartSlatAng.Rows[i]["COLOUR"].ToString();
        //                nOrderDetail.QTY = SmartSlatAng.Rows[i][SmartSlatAng.Columns[5].ColumnName].ToString();
        //                nOrderDetails.Add(nOrderDetail);
        //            }


        //            if ((SmartSlatAng.Rows[i][SmartSlatAng.Columns[7].ColumnName].ToString() != "") &&
        //                    (SmartSlatAng.Rows[i][SmartSlatAng.Columns[8].ColumnName].ToString() != ""))
        //            {
        //                NewOrderDetail nOrderDetail = new NewOrderDetail();
        //                nOrderDetail.TYPE = "SmartSlatAng";
        //                nOrderDetail.LENGTH = SmartSlatAng.Rows[i][SmartSlatAng.Columns[8].ColumnName].ToString();
        //                nOrderDetail.COLOUR = SmartSlatAng.Rows[i]["COLOUR"].ToString();
        //                nOrderDetail.QTY = SmartSlatAng.Rows[i][SmartSlatAng.Columns[7].ColumnName].ToString();
        //                nOrderDetails.Add(nOrderDetail);
        //            }
        //        }
        //    }

        //    return nOrderDetails;
        //}


        //public static IList<ItemDescription> CreateItemDescription()
        //{
        //    IList<ItemDescription> itemDescriptions = new List<ItemDescription>();


        //    if ((GramLine != null) && (GramLine.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < GramLine.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < GramLine.Columns.Count; j++)
        //            {
        //                if (GramLine.Rows[i][GramLine.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    ItemDescription itemDescription = new ItemDescription();
        //                    itemDescription.TYPE = "GramLine";
        //                    itemDescription.LENGTH = GramLine.Columns[j].ColumnName;
        //                    itemDescription.COLOUR = GramLine.Rows[i]["COLOUR"].ToString();
        //                    itemDescription.QTY = GramLine.Rows[i][GramLine.Columns[j].ColumnName].ToString();
        //                    itemDescriptions.Add(itemDescription);
        //                }
        //            }
        //        }
        //    }

        //    if ((ColorLine != null) && (ColorLine.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < ColorLine.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < ColorLine.Columns.Count; j++)
        //            {
        //                if (ColorLine.Rows[i][ColorLine.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    ItemDescription itemDescription = new ItemDescription();
        //                    itemDescription.TYPE = "ColorLine";
        //                    itemDescription.LENGTH = ColorLine.Columns[j].ColumnName;
        //                    itemDescription.COLOUR = ColorLine.Rows[i]["COLOUR"].ToString();
        //                    itemDescription.QTY = ColorLine.Rows[i][ColorLine.Columns[j].ColumnName].ToString();
        //                    itemDescriptions.Add(itemDescription);
        //                }
        //            }
        //        }
        //    }

        //    if ((RailsPosts != null) && (RailsPosts.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < RailsPosts.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < RailsPosts.Columns.Count; j++)
        //            {
        //                if (RailsPosts.Rows[i][RailsPosts.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    ItemDescription itemDescription = new ItemDescription();
        //                    itemDescription.TYPE = "RailsPosts";
        //                    itemDescription.LENGTH = RailsPosts.Columns[j].ColumnName;
        //                    itemDescription.COLOUR = RailsPosts.Rows[i]["COLOUR"].ToString();
        //                    itemDescription.QTY = RailsPosts.Rows[i][RailsPosts.Columns[j].ColumnName].ToString();
        //                    itemDescriptions.Add(itemDescription);
        //                }
        //            }
        //        }
        //    }


        //    if ((SquarePosts50 != null) && (SquarePosts50.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < SquarePosts50.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SquarePosts50.Columns.Count; j++)
        //            {
        //                if (SquarePosts50.Rows[i][SquarePosts50.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    ItemDescription itemDescription = new ItemDescription();
        //                    itemDescription.TYPE = "SquarePosts50";
        //                    itemDescription.LENGTH = SquarePosts50.Columns[j].ColumnName;
        //                    itemDescription.COLOUR = SquarePosts50.Rows[i]["COLOUR"].ToString();
        //                    itemDescription.QTY = SquarePosts50.Rows[i][SquarePosts50.Columns[j].ColumnName].ToString();
        //                    itemDescriptions.Add(itemDescription);
        //                }
        //            }
        //        }
        //    }


        //    if ((SquarePosts60 != null) && (SquarePosts60.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < SquarePosts60.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SquarePosts60.Columns.Count; j++)
        //            {
        //                if (SquarePosts60.Rows[i][SquarePosts60.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    ItemDescription itemDescription = new ItemDescription();
        //                    itemDescription.TYPE = "SquarePosts60";
        //                    itemDescription.LENGTH = SquarePosts60.Columns[j].ColumnName;
        //                    itemDescription.COLOUR = SquarePosts60.Rows[i]["COLOUR"].ToString();
        //                    itemDescription.QTY = SquarePosts60.Rows[i][SquarePosts60.Columns[j].ColumnName].ToString();
        //                    itemDescriptions.Add(itemDescription);
        //                }
        //            }
        //        }
        //    }


        //    if ((SquarePosts65 != null) && (SquarePosts65.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < SquarePosts65.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < SquarePosts65.Columns.Count; j++)
        //            {
        //                if (SquarePosts65.Rows[i][SquarePosts65.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    ItemDescription itemDescription = new ItemDescription();
        //                    itemDescription.TYPE = "SquarePosts65";
        //                    itemDescription.LENGTH = SquarePosts65.Columns[j].ColumnName;
        //                    itemDescription.COLOUR = SquarePosts65.Rows[i]["COLOUR"].ToString();
        //                    itemDescription.QTY = SquarePosts65.Rows[i][SquarePosts65.Columns[j].ColumnName].ToString();
        //                    itemDescriptions.Add(itemDescription);
        //                }
        //            }
        //        }
        //    }


        //    if ((Lattice != null) && (Lattice.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < Lattice.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < Lattice.Columns.Count; j++)
        //            {
        //                if (Lattice.Rows[i][Lattice.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    ItemDescription itemDescription = new ItemDescription();
        //                    itemDescription.TYPE = "Lattice";
        //                    itemDescription.LENGTH = Lattice.Columns[j].ColumnName;
        //                    itemDescription.COLOUR = Lattice.Rows[i]["COLOUR"].ToString();
        //                    itemDescription.QTY = Lattice.Rows[i][Lattice.Columns[j].ColumnName].ToString();
        //                    itemDescriptions.Add(itemDescription);
        //                }
        //            }
        //        }
        //    }


        //    if ((Plinths != null) && (Plinths.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < Plinths.Rows.Count; i++)
        //        {
        //            for (int j = 1; j < Plinths.Columns.Count; j++)
        //            {
        //                if (Plinths.Rows[i][Plinths.Columns[j].ColumnName].ToString() != "")
        //                {
        //                    ItemDescription itemDescription = new ItemDescription();
        //                    itemDescription.TYPE = "Plinths";
        //                    itemDescription.LENGTH = Plinths.Columns[j].ColumnName;
        //                    itemDescription.COLOUR = Plinths.Rows[i]["COLOUR"].ToString();
        //                    itemDescription.QTY = Plinths.Rows[i][Plinths.Columns[j].ColumnName].ToString();
        //                    itemDescriptions.Add(itemDescription);
        //                }
        //            }
        //        }
        //    }


        //    if ((SmartSlatAng != null) && (SmartSlatAng.Rows.Count > 0))
        //    {
        //        for (int i = 0; i < SmartSlatAng.Rows.Count; i++)
        //        {
        //            if ((SmartSlatAng.Rows[i][SmartSlatAng.Columns[1].ColumnName].ToString() != "") &&
        //                    (SmartSlatAng.Rows[i][SmartSlatAng.Columns[2].ColumnName].ToString() != ""))
        //            {
        //                ItemDescription itemDescription = new ItemDescription();
        //                itemDescription.TYPE = "SmartSlatAng";
        //                itemDescription.LENGTH = SmartSlatAng.Rows[i][SmartSlatAng.Columns[2].ColumnName].ToString();
        //                itemDescription.COLOUR = SmartSlatAng.Rows[i]["COLOUR"].ToString();
        //                itemDescription.QTY = SmartSlatAng.Rows[i][SmartSlatAng.Columns[1].ColumnName].ToString();
        //                itemDescriptions.Add(itemDescription);
        //            }


        //            if ((SmartSlatAng.Rows[i][SmartSlatAng.Columns[3].ColumnName].ToString() != "") &&
        //                    (SmartSlatAng.Rows[i][SmartSlatAng.Columns[4].ColumnName].ToString() != ""))
        //            {
        //                ItemDescription itemDescription = new ItemDescription();
        //                itemDescription.TYPE = "SmartSlatAng";
        //                itemDescription.LENGTH = SmartSlatAng.Rows[i][SmartSlatAng.Columns[4].ColumnName].ToString();
        //                itemDescription.COLOUR = SmartSlatAng.Rows[i]["COLOUR"].ToString();
        //                itemDescription.QTY = SmartSlatAng.Rows[i][SmartSlatAng.Columns[3].ColumnName].ToString();
        //                itemDescriptions.Add(itemDescription);
        //            }


        //            if ((SmartSlatAng.Rows[i][SmartSlatAng.Columns[5].ColumnName].ToString() != "") &&
        //                    (SmartSlatAng.Rows[i][SmartSlatAng.Columns[6].ColumnName].ToString() != ""))
        //            {
        //                ItemDescription itemDescription = new ItemDescription();
        //                itemDescription.TYPE = "SmartSlatAng";
        //                itemDescription.LENGTH = SmartSlatAng.Rows[i][SmartSlatAng.Columns[6].ColumnName].ToString();
        //                itemDescription.COLOUR = SmartSlatAng.Rows[i]["COLOUR"].ToString();
        //                itemDescription.QTY = SmartSlatAng.Rows[i][SmartSlatAng.Columns[5].ColumnName].ToString();
        //                itemDescriptions.Add(itemDescription);
        //            }


        //            if ((SmartSlatAng.Rows[i][SmartSlatAng.Columns[7].ColumnName].ToString() != "") &&
        //                    (SmartSlatAng.Rows[i][SmartSlatAng.Columns[8].ColumnName].ToString() != ""))
        //            {
        //                ItemDescription itemDescription = new ItemDescription();
        //                itemDescription.TYPE = "SmartSlatAng";
        //                itemDescription.LENGTH = SmartSlatAng.Rows[i][SmartSlatAng.Columns[8].ColumnName].ToString();
        //                itemDescription.COLOUR = SmartSlatAng.Rows[i]["COLOUR"].ToString();
        //                itemDescription.QTY = SmartSlatAng.Rows[i][SmartSlatAng.Columns[7].ColumnName].ToString();
        //                itemDescriptions.Add(itemDescription);
        //            }
        //        }
        //    }

        //    return itemDescriptions;
        //}
    }
}