// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using str = System.String;
using X = System.Xml.Linq.XElement;
using Debug = System.Diagnostics.Debug;
using System.Diagnostics;
public class Program {
    Dictionary<str, str> pages = new();
    public static void Main(str[] args) { 
        var head = new Head(
            title: "HTMLSharp",
            meta: new(
                og_title:       "HTMLSharp",
                og_description: "Static HTML generator in C#",
                og_url:         "https://INeedAUniqueUsername.github.io/HTMLSharp.html"
                ),
            styles:new(){
                new("*", new(fontFamily: "Consolas", fontStyle: "plain"))
            }
        );
        var body = new Body(new Style(color: "white", backgroundColor: "black"),
            new p("aaaaaa")
        );
        var str =
@$"<html>
{head.GetX()}
{((IElement)body).GetX()}
</html>";
        Console.WriteLine(str);
        File.WriteAllText("index.html", str);

        Process.Start(new ProcessStartInfo(){
            FileName = "index.html",
            UseShellExecute = true
        });
    }
}
public record Head(str title, Meta? meta = null, List<Selector>? styles = null) {
    public X GetX() =>
        new X("head",
            new X("title", title),
            meta?.GetX(),
            new X("style", styles?.Select(s => s.GetStr()))
        );
    
}
public static class SElement {
    public static IElement AddChildren(this IElement e, params IElement[] children) {
        e.children.AddRange(children);
        return e;
    }
    //public static str Tag(str tag, str content) => content?.Any() != true ? "" : $"<{tag}>{content}
}
public interface IElement {
    public str tag { get; }
    public Dictionary<str, str> attributes => new();
    public List<IElement> children { get; }
    public IElement AddChildren(params IElement[] children) {
        this.children.AddRange(children);
        return this;
    }
    public Style GetStyle() => null;
    public str GetText() => "";
    public str GetChildrenstr() => str.Join('\n', children.Select(c => c.GetX())) ?? "";
    public X GetX() {
        
        var e = new X(tag);
        if(GetStyle()?.GetStr() is str s && s.Any()){
            e.SetAttributeValue("style", s);
        }
        foreach((var key, var value) in attributes){
            if(value == null) continue;
            e.SetAttributeValue(key, value);
        }
        if(GetText() is str t && t.Any()) {
            e.SetValue(GetText());
        }
        children.ForEach(c => e.Add(c.GetX()));
        return e;
        /*
        var open = tag;
        if(GetStyle()?.GetStr() is str s && s.Any()){
            open += $" style=\"{s}\"";
        }
        var content = (GetText() + GetChildrenstr());
        if(content.Any()){
            return $"<{open}>{content}</{tag}>";
        }
        return $"<{open}/>";
        */
    }
}
public record Body(Style style = null) : IElement {
    public Body(Style style = null, params IElement[] children) : this(style){
        this.children.AddRange(children);
    }
    public Body(params IElement[] children) : this((Style)null){
        this.children.AddRange(children);
    }
    public str tag => "body";
    public List<IElement> children { get; } = new();
    public Style GetStyle() => style;
}

public record Div(Style style = null) : IElement {
    public Div(Style style = null, params IElement[] children) : this(style){
        this.children.AddRange(children);
    } 
    public str tag => "div";
    public List<IElement> children { get; } = new();
    public Style GetStyle() => style;
}

public record A(string href, Style style = null) : IElement {
    public A(Style style, string href) : this(href, style){}
    public str tag => "a";
    public Dictionary<str, str> attributes => new() {
        ["href"] = href
    };
    public List<IElement> children { get; } = new();
    public Style GetStyle() => style;
}
public record img(int? width, int? height, Style style = null) : IElement {
    public str tag => $"img";
    
    public Dictionary<str, str> attributes => new() {
        ["width"] = width?.ToString(),
        ["height"] = height?.ToString()
    };
    public Style GetStyle() => style;
    public List<IElement> children { get; } = new();
}
public record p(str text, Style style = null) : IElement {
    public p(Style style, str text) : this(text, style){}
    public str tag => "p";
    public Style GetStyle() => style;
    public str GetText() => text;
    public List<IElement> children { get; } = new();
}
public record h1(str text, Style style = null) : IElement {
    public h1(Style style, str text) : this(text, style){}
    public str tag => "h1";
    public Style GetStyle() => style;
    public str GetText() => text;
    public List<IElement> children { get; } = new();
}
public record Selector(str selector, Style style) {
    public str GetStr() => $"{selector} {{\n{style.GetStr()}\n}}";
}
public record Style() {
    ///<summary>aaaaaaa</summary>
    ///<param name="width">000000</param>
    ///
    public Style(
        str? width = null,
        str? height = null,
        str? marginLeft = null,
        str? marginTop = null,
        str? marginRight = null,
        str? marginBottom = null,
        bool centerHorizontal = false,
        str? font = null,
        str? fontFamily = null,
        str? fontSize = null,
        str? fontStyle = null,
        str? color=null,
        str? backgroundColor=null
    ) : this() {
        this.width=width;
        this.height=height;
        this.marginLeft=marginLeft;
        this.marginTop=marginTop;
        this.marginRight=marginRight;
        this.marginBottom=marginBottom;
        this.font = font;
        this.fontFamily=fontFamily;
        this.fontSize=fontSize;
        this.fontStyle=fontStyle;
        this.color = color;
        this.backgroundColor = backgroundColor;
        if(centerHorizontal){
            Debug.Assert(marginLeft == null);
            Debug.Assert(marginRight == null);
            marginLeft = marginRight = "auto";
        }
    }
    public str? width, height,
        marginLeft, marginTop, marginRight, marginBottom,
        display,
        font, fontFamily, fontSize, fontStyle,
        color, backgroundColor;
    public str GetStr() {
        StringBuilder result = new();
        var d = new Dictionary<string, string?>() {
            ["width"] = width,
            ["height"] = height,
            ["margin-left"] = marginLeft,
            ["margin-top"] = marginTop,
            ["margin-right"] = marginRight,
            ["margin-bottom"] = marginBottom,
            ["display"] = display,
            ["font"] = font,
            ["font-family"] = fontFamily,
            ["font-size"] = fontSize,
            ["font-style"] = fontStyle,
            ["color"] = color,
            ["background-color"] = backgroundColor
        };
        foreach((var key, var value) in d){
            if(value?.Any() != true) continue;
            result.Append($"{key}:{value}; ");
        }
        return result.ToString();
    }
}
public static class Margin {
    public static string auto = "auto";
}
///<summary>Defines meta tags in the head </summary>
public record Meta() {
    Dictionary<str, str?> d=new();
    
    ///<summary>Defines the OpenGraph preview and Twitter preview</summary>
    public Meta(
        str? og_title = null,
        str? og_type = "website",
        str? og_url = null,
        str? og_description = null,
        str? twitter_card = null
    ) : this() {
        d=new(){
            ["og:title"]=og_title,
            ["og:type"]=og_type,
            ["og:url"] = og_url,
            ["og:description"] = og_description,
            ["twitter:card"] = twitter_card
            };
    }
    public List<X> GetX() {
        var e = new List<X>();
        foreach((var key, var value) in d){
            if(value==null) continue;
            var x = new X("meta");
            x.SetAttributeValue("property", key);
            x.SetAttributeValue("content", value);
            e.Add(x);
        }
        return e;
    }
}