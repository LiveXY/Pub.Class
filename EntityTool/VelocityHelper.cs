using System;
using System.Web;
using System.IO;
using System.Text;
using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;
using Commons.Collections;
using Pub.Class;
using System.Collections.Generic;

/// <summary>
/// NVelocity模板工具类 VelocityHelper
/// </summary>
public class VelocityHelper {
    private VelocityEngine velocity = null;
    private IContext context = null;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="templatePath">模板文件夹路径</param>
    public VelocityHelper(string templatePath) {
        Init(templatePath);
    }

    /// <summary>
    /// 无参数构造函数
    /// </summary>
    public VelocityHelper() { ;}

    /// <summary>
    /// 初始话NVelocity模块
    /// </summary>
    /// <param name="templatePath">模板文件夹路径</param>
    public void Init(string templatePath) {
        //创建VelocityEngine实例对象
        velocity = new VelocityEngine();

        //使用设置初始化VelocityEngine
        ExtendedProperties props = new ExtendedProperties();
        props.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");
        string path = "".GetMapPath() + templatePath;
        props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, path);

        props.AddProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
        props.AddProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");

        //props.SetProperty(RuntimeConstants.RESOURCE_LOADER, "assembly");
        //props.SetProperty("assembly.resource.loader.assembly", new List<string>() { "Pub.Class" });

        //是否缓存
        props.AddProperty("file.resource.loader.modificationCheckInterval", (Int64)30);    //缓存时间(秒)

        velocity.Init(props);

        //为模板变量赋值
        context = new VelocityContext();
    }

    /// <summary>
    /// 给模板变量赋值
    /// </summary>
    /// <param name="key">模板变量</param>
    /// <param name="value">模板变量值</param>
    public void Put(string key, object value) {
        if (context.IsNull()) {
            context = new VelocityContext();
        }
        context.Put(key, value);
    }

    /// <summary>
    /// 根据指定的内容类型显示模板
    /// </summary>
    /// <param name="templateFileName">模板文件名</param>
    public string Display(string templateFileName) {
        //从文件中读取模板
        Template template = velocity.GetTemplate(templateFileName);

        //合并模板
        StringWriter writer = new StringWriter();
        template.Merge(context, writer);

        return writer.ToString();
    }
}
