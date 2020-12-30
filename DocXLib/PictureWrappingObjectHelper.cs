// Decompiled with JetBrains decompiler
// Type: Xceed.Document.NET.PictureWrappingObjectHelper
// Assembly: Xceed.Document.NETStandard, Version=1.7.20371.21580, Culture=neutral, PublicKeyToken=ba83ff368b7563c6
// MVID: DA30F741-A666-4EFA-B79F-CC64891B04D2
// Assembly location: C:\Program Files (x86)\Xceed\Xceed Words for .NET v1.7\Bin\NETStandard\Xceed.Document.NETStandard.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace Xceed.Document.NET
{
  internal class PictureWrappingObjectHelper : 
    WrappingObjectHelper,
    IPictureWrappingObject,
    IWrappingObject
  {
    private PictureWrappingStyle _wrappingStyle;
    private PictureWrapText _wrapText;
    private List<Point> _wrapPolygon;

    public PictureWrappingStyle WrappingStyle
    {
      get => this._wrappingStyle;
      set
      {
        this._wrappingStyle = value;
        this.UpdateTextWrapping();
      }
    }

    public PictureWrapText WrapText
    {
      get => this._wrapText;
      set
      {
        this._wrapText = value;
        this.UpdateTextWrapping();
      }
    }

    public List<Point> WrapPolygon
    {
      get => this._wrapPolygon;
      set
      {
        if (value != null)
        {
          if (value.Count < 3)
            throw new InvalidOperationException("At least 3 Points are necessary in order to use the WrapPanel.");
          if (value.First<Point>() != value.Last<Point>())
            value.Add(value.First<Point>());
        }
        this._wrapPolygon = value;
        this.UpdateTextWrapping();
      }
    }

    internal override void UpdateHorizontalAlignment()
    {
      if (this.WrappingStyle == PictureWrappingStyle.WrapInLineWithText)
        return;
      XElement xelement1 = this.GetXml().Descendants(XName.Get("anchor", Xceed.Document.NET.Document.wp.NamespaceName)).FirstOrDefault<XElement>();
      if (xelement1 == null)
        return;
      XElement xelement2 = new XElement(XName.Get("positionH", Xceed.Document.NET.Document.wp.NamespaceName));
      xelement2.SetAttributeValue((XName) "relativeFrom", (object) this.GetHorizontalRelativeFrom());
      if (this.HorizontalAlignment != WrappingHorizontalAlignment.None)
      {
        XElement xelement3 = new XElement(XName.Get("align", Xceed.Document.NET.Document.wp.NamespaceName), (object) this.GetHorizontalAlignment());
        xelement2.Add((object) xelement3);
      }
      else
      {
        XElement xelement3 = new XElement(XName.Get("posOffset", Xceed.Document.NET.Document.wp.NamespaceName), (object) (this.HorizontalOffset * 12700.0));
        xelement2.Add((object) xelement3);
      }
      xelement1.Element(XName.Get("positionH", Xceed.Document.NET.Document.wp.NamespaceName)).ReplaceWith((object) xelement2);
    }

    internal override void UpdateVerticalAlignment()
    {
      if (this.WrappingStyle == PictureWrappingStyle.WrapInLineWithText)
        return;
      XElement xelement1 = this.GetXml().Descendants(XName.Get("anchor", Xceed.Document.NET.Document.wp.NamespaceName)).FirstOrDefault<XElement>();
      if (xelement1 == null)
        return;
      XElement xelement2 = new XElement(XName.Get("positionV", Xceed.Document.NET.Document.wp.NamespaceName));
      xelement2.SetAttributeValue((XName) "relativeFrom", (object) this.GetVerticalRelativeFrom());
      if (this.VerticalAlignment != WrappingVerticalAlignment.None)
      {
        XElement xelement3 = new XElement(XName.Get("align", Xceed.Document.NET.Document.wp.NamespaceName), (object) this.GetVerticalAlignment());
        xelement2.Add((object) xelement3);
      }
      else
      {
        XElement xelement3 = new XElement(XName.Get("posOffset", Xceed.Document.NET.Document.wp.NamespaceName), (object) (this.VerticalOffset * 12700.0));
        xelement2.Add((object) xelement3);
      }
      xelement1.Element(XName.Get("positionV", Xceed.Document.NET.Document.wp.NamespaceName)).ReplaceWith((object) xelement2);
    }

    internal override void UpdateDistanceFromText()
    {
      if (this.WrappingStyle == PictureWrappingStyle.WrapInLineWithText || this.WrappingStyle == PictureWrappingStyle.WrapBehindText || this.WrappingStyle == PictureWrappingStyle.WrapInFrontOfText)
        return;
      XElement xelement = this.GetXml().Descendants(XName.Get("anchor", Xceed.Document.NET.Document.wp.NamespaceName)).FirstOrDefault<XElement>();
      if (xelement == null)
        return;
      if (this.WrappingStyle != PictureWrappingStyle.WrapTight && this.WrappingStyle != PictureWrappingStyle.WrapThrough)
      {
        xelement.SetAttributeValue((XName) "distT", (object) (this.DistanceFromTextTop * 12700.0));
        xelement.SetAttributeValue((XName) "distB", (object) (this.DistanceFromTextBottom * 12700.0));
      }
      if (this.WrappingStyle == PictureWrappingStyle.WrapTopAndBottom)
        return;
      xelement.SetAttributeValue((XName) "distL", (object) (this.DistanceFromTextLeft * 12700.0));
      xelement.SetAttributeValue((XName) "distR", (object) (this.DistanceFromTextRight * 12700.0));
    }

    internal override string GetHorizontalRelativeFrom()
    {
      if (this.HorizontalAlignment != WrappingHorizontalAlignment.None)
      {
        switch (this.HorizontalAlignment)
        {
          case WrappingHorizontalAlignment.LeftRelativeToMargin:
          case WrappingHorizontalAlignment.CenteredRelativeToMargin:
          case WrappingHorizontalAlignment.RightRelativeToMargin:
          case WrappingHorizontalAlignment.InsideOfMargin:
          case WrappingHorizontalAlignment.OutsideOfMargin:
            return "margin";
          case WrappingHorizontalAlignment.LeftRelativeToPage:
          case WrappingHorizontalAlignment.CenteredRelativeToPage:
          case WrappingHorizontalAlignment.RightRelativeToPage:
          case WrappingHorizontalAlignment.InsideOfPage:
          case WrappingHorizontalAlignment.OutsideOfPage:
            return "page";
          case WrappingHorizontalAlignment.LeftRelativeToColumn:
          case WrappingHorizontalAlignment.CenteredRelativeToColumn:
          case WrappingHorizontalAlignment.RightRelativeToColumn:
            return "column";
          case WrappingHorizontalAlignment.LeftRelativeToCharacter:
          case WrappingHorizontalAlignment.CenteredRelativeToCharacter:
          case WrappingHorizontalAlignment.RightRelativeToCharacter:
            return "character";
          case WrappingHorizontalAlignment.LeftRelativeToLeftMargin:
          case WrappingHorizontalAlignment.CenteredRelativeToLeftMargin:
          case WrappingHorizontalAlignment.RightRelativeToLeftMargin:
            return "leftMargin";
          case WrappingHorizontalAlignment.LeftRelativeToRightMargin:
          case WrappingHorizontalAlignment.CenteredRelativeToRightMargin:
          case WrappingHorizontalAlignment.RightRelativeToRightMargin:
            return "rightMargin";
          case WrappingHorizontalAlignment.LeftRelativeToInsideMargin:
          case WrappingHorizontalAlignment.CenteredRelativeToInsideMargin:
          case WrappingHorizontalAlignment.RightRelativeToInsideMargin:
            return "insideMargin";
          case WrappingHorizontalAlignment.LeftRelativeToOutsideMargin:
          case WrappingHorizontalAlignment.CenteredRelativeToOutsideMargin:
          case WrappingHorizontalAlignment.RightRelativeToOutsideMargin:
            return "outsideMargin";
          default:
            return "margin";
        }
      }
      else
      {
        switch (this.HorizontalOffsetAlignmentFrom)
        {
          case WrappingHorizontalOffsetAlignmentFrom.Margin:
            return "margin";
          case WrappingHorizontalOffsetAlignmentFrom.Page:
            return "page";
          case WrappingHorizontalOffsetAlignmentFrom.Column:
            return "column";
          case WrappingHorizontalOffsetAlignmentFrom.Character:
            return "character";
          case WrappingHorizontalOffsetAlignmentFrom.LeftMargin:
            return "leftMargin";
          case WrappingHorizontalOffsetAlignmentFrom.RightMargin:
            return "rightMargin";
          case WrappingHorizontalOffsetAlignmentFrom.InsideMargin:
            return "insideMargin";
          case WrappingHorizontalOffsetAlignmentFrom.OutSideMargin:
            return "outsideMargin";
          default:
            return "margin";
        }
      }
    }

    internal override string GetVerticalRelativeFrom()
    {
      if (this.VerticalAlignment != WrappingVerticalAlignment.None)
      {
        switch (this.VerticalAlignment)
        {
          case WrappingVerticalAlignment.TopRelativeToMargin:
          case WrappingVerticalAlignment.CenteredRelativeToMargin:
          case WrappingVerticalAlignment.BottomRelativeToMargin:
          case WrappingVerticalAlignment.InsideRelativeToMargin:
          case WrappingVerticalAlignment.OutsideRelativeToMargin:
            return "margin";
          case WrappingVerticalAlignment.TopRelativeToPage:
          case WrappingVerticalAlignment.CenteredRelativeToPage:
          case WrappingVerticalAlignment.BottomRelativeToPage:
          case WrappingVerticalAlignment.InsideRelativeToPage:
          case WrappingVerticalAlignment.OutsideRelativeToPage:
            return "page";
          case WrappingVerticalAlignment.TopRelativeToLine:
          case WrappingVerticalAlignment.CenteredRelativeToLine:
          case WrappingVerticalAlignment.BottomRelativeToLine:
          case WrappingVerticalAlignment.InsideRelativeToLine:
          case WrappingVerticalAlignment.OutsideRelativeToLine:
            return "line";
          case WrappingVerticalAlignment.TopRelativeToTopMargin:
          case WrappingVerticalAlignment.CenteredRelativeToTopMargin:
          case WrappingVerticalAlignment.BottomRelativeToTopMargin:
          case WrappingVerticalAlignment.InsideRelativeToTopMargin:
          case WrappingVerticalAlignment.OutsideRelativeToTopMargin:
            return "topMargin";
          case WrappingVerticalAlignment.TopRelativeToBottomMargin:
          case WrappingVerticalAlignment.CenteredRelativeToBottomMargin:
          case WrappingVerticalAlignment.BottomRelativeToBottomMargin:
          case WrappingVerticalAlignment.InsideRelativeToBottomMargin:
          case WrappingVerticalAlignment.OutsideRelativeToBottomMargin:
            return "bottomMargin";
          case WrappingVerticalAlignment.TopRelativeToInsideMargin:
          case WrappingVerticalAlignment.CenteredRelativeToInsideMargin:
          case WrappingVerticalAlignment.BottomRelativeToInsideMargin:
          case WrappingVerticalAlignment.InsideRelativeToInsideMargin:
          case WrappingVerticalAlignment.OutsideRelativeToInsideMargin:
            return "insideMargin";
          case WrappingVerticalAlignment.TopRelativeToOutsideMargin:
          case WrappingVerticalAlignment.CenteredRelativeToOutsideMargin:
          case WrappingVerticalAlignment.BottomRelativeToOutsideMargin:
          case WrappingVerticalAlignment.InsideRelativeToOutsideMargin:
          case WrappingVerticalAlignment.OutsideRelativeToOutsideMargin:
            return "outsideMargin";
          default:
            return "margin";
        }
      }
      else
      {
        switch (this.VerticalOffsetAlignmentFrom)
        {
          case WrappingVerticalOffsetAlignmentFrom.Margin:
            return "margin";
          case WrappingVerticalOffsetAlignmentFrom.Page:
            return "page";
          case WrappingVerticalOffsetAlignmentFrom.Paragraph:
            return "paragraph";
          case WrappingVerticalOffsetAlignmentFrom.Line:
            return "line";
          case WrappingVerticalOffsetAlignmentFrom.TopMargin:
            return "topMargin";
          case WrappingVerticalOffsetAlignmentFrom.BottomMargin:
            return "bottomMargin";
          case WrappingVerticalOffsetAlignmentFrom.InsideMargin:
            return "insideMargin";
          case WrappingVerticalOffsetAlignmentFrom.OutSideMargin:
            return "outsideMargin";
          default:
            return "margin";
        }
      }
    }

    internal PictureWrappingObjectHelper(XElement xml)
      : base(xml)
    {
    }

    private void UpdateTextWrapping()
    {
      if (this.WrappingStyle == PictureWrappingStyle.WrapInLineWithText)
      {
        XElement xelement1 = this.GetXml().Descendants(XName.Get("anchor", Xceed.Document.NET.Document.wp.NamespaceName)).FirstOrDefault<XElement>();
        if (xelement1 == null)
          return;
        xelement1.Name = XName.Get("inline", Xceed.Document.NET.Document.wp.NamespaceName);
        XElement xelement2 = new XElement(XName.Get("wrapNone", Xceed.Document.NET.Document.wp.NamespaceName));
        xelement1.Elements().FirstOrDefault<XElement>((Func<XElement, bool>) (x => x.Name.LocalName.StartsWith("wrap"))).ReplaceWith((object) xelement2);
      }
      else
      {
        XElement xelement1 = this.GetXml().Descendants(XName.Get("inline", Xceed.Document.NET.Document.wp.NamespaceName)).FirstOrDefault<XElement>();
        if (xelement1 != null)
          xelement1.Name = XName.Get("anchor", Xceed.Document.NET.Document.wp.NamespaceName);
        XElement xelement2 = this.GetXml().Descendants(XName.Get("anchor", Xceed.Document.NET.Document.wp.NamespaceName)).FirstOrDefault<XElement>();
        if (xelement2 == null)
          return;
        XElement xelement3;
        switch (this.WrappingStyle)
        {
          case PictureWrappingStyle.WrapTopAndBottom:
            xelement3 = new XElement(XName.Get("wrapTopAndBottom", Xceed.Document.NET.Document.wp.NamespaceName));
            break;
          case PictureWrappingStyle.WrapSquare:
            xelement3 = new XElement(XName.Get("wrapSquare", Xceed.Document.NET.Document.wp.NamespaceName));
            xelement3.SetAttributeValue((XName) "wrapText", (object) this.WrapText.ToString());
            break;
          case PictureWrappingStyle.WrapTight:
            xelement3 = new XElement(XName.Get("wrapTight", Xceed.Document.NET.Document.wp.NamespaceName));
            xelement3.SetAttributeValue((XName) "wrapText", (object) this.WrapText.ToString());
            xelement3.Add((object) this.CreateWrapPolygonNode());
            break;
          case PictureWrappingStyle.WrapThrough:
            xelement3 = new XElement(XName.Get("wrapThrough", Xceed.Document.NET.Document.wp.NamespaceName));
            xelement3.SetAttributeValue((XName) "wrapText", (object) this.WrapText.ToString());
            xelement3.Add((object) this.CreateWrapPolygonNode());
            break;
          case PictureWrappingStyle.WrapBehindText:
            xelement3 = new XElement(XName.Get("wrapNone", Xceed.Document.NET.Document.wp.NamespaceName));
            xelement2.SetAttributeValue((XName) "behindDoc", (object) "1");
            break;
          case PictureWrappingStyle.WrapInFrontOfText:
            xelement3 = new XElement(XName.Get("wrapNone", Xceed.Document.NET.Document.wp.NamespaceName));
            xelement2.SetAttributeValue((XName) "behindDoc", (object) "0");
            break;
          default:
            throw new NotSupportedException("Wrapping Style is not supported");
        }
        xelement2.Elements().FirstOrDefault<XElement>((Func<XElement, bool>) (x => x.Name.LocalName.StartsWith("wrap"))).ReplaceWith((object) xelement3);
      }
    }

    private XElement CreateWrapPolygonNode()
    {
      string str = "1";
      if (this.WrapPolygon == null)
      {
        this._wrapPolygon = new List<Point>();
        this._wrapPolygon.Add(new Point(0, 0));
        this._wrapPolygon.Add(new Point(0, 21600));
        this._wrapPolygon.Add(new Point(21600, 21600));
        this._wrapPolygon.Add(new Point(21600, 0));
        this._wrapPolygon.Add(new Point(0, 0));
        str = "0";
      }
      XElement xelement1 = new XElement(XName.Get("wrapPolygon", Xceed.Document.NET.Document.wp.NamespaceName));
      xelement1.SetAttributeValue((XName) "edited", (object) str);
      XElement xelement2 = new XElement(XName.Get("start", Xceed.Document.NET.Document.wp.NamespaceName));
      XElement xelement3 = xelement2;
      XName name1 = (XName) "x";
      Point point = this.WrapPolygon[0];
      // ISSUE: variable of a boxed type
      __Boxed<int> x1 = (ValueType) point.X;
      xelement3.SetAttributeValue(name1, (object) x1);
      XElement xelement4 = xelement2;
      XName name2 = (XName) "y";
      point = this.WrapPolygon[0];
      // ISSUE: variable of a boxed type
      __Boxed<int> y1 = (ValueType) point.Y;
      xelement4.SetAttributeValue(name2, (object) y1);
      xelement1.Add((object) xelement2);
      for (int index = 1; index < this.WrapPolygon.Count; ++index)
      {
        XElement xelement5 = new XElement(XName.Get("lineTo", Xceed.Document.NET.Document.wp.NamespaceName));
        XElement xelement6 = xelement5;
        XName name3 = (XName) "x";
        point = this.WrapPolygon[index];
        // ISSUE: variable of a boxed type
        __Boxed<int> x2 = (ValueType) point.X;
        xelement6.SetAttributeValue(name3, (object) x2);
        XElement xelement7 = xelement5;
        XName name4 = (XName) "y";
        point = this.WrapPolygon[index];
        // ISSUE: variable of a boxed type
        __Boxed<int> y2 = (ValueType) point.Y;
        xelement7.SetAttributeValue(name4, (object) y2);
        xelement1.Add((object) xelement5);
      }
      return xelement1;
    }
  }
}
