using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using CodeSmith.Engine;
using CodeSmith.BaseTemplates;
using CodeSmith.CustomProperties;
using SchemaExplorer;

namespace CSBatisBuilder {

    #region Enums

    /// <summary>Controls the access modifier of Property/MemberVariable generation.</summary>
    public enum MemberAccess { Public, Protected, ProtectedInternal, Internal, Private }

    /// <summary><c>IdPattern</c> specifies the pattern of statement id.</summary>
    public enum StmtIdPattern { Local, Classified, NamespaceClassified }

    /// <summary>The enum of database vendors.</summary>
    /// <remarks>Each implemenator class of <see cref="IDbVendor"/> should have a corresponding value in this enum. 
    /// <para>Currently only Micorsoft SQL Server (<see cref="MSSqlDbVendor"/>) implemented.</para></remarks>
    public enum DbVendorId { SQLServer }

    #endregion

    #region Categories

    /// <summary>String constants for <see cref="CategoryAttribute"/>.</summary>
    public class Categories {

        public const String Data = "Data";

        public const String Options = "Options";

        public const String Output = "Output";

        public const String DebugInfo = "DebugInfo";

    }

    #endregion

    #region ClassOption

    /// <summary><c>ClassOption</c> is the base class for customizing a C# type.</summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public abstract class ClassOption {

        #region Namespace

        private String m_Namespace;

        [Optional]
        [Description("(Recommanded) The namespace of the generated class.")]
        public String Namespace {
            get { return m_Namespace; }
            set { m_Namespace = (value != null) ? value.Trim() : String.Empty; }
        }

        #endregion

        #region Assembly

        private String m_Assembly;

        [Optional]
        [Description("The assembly name, defult to the Namespace.")]
        public String Assembly {
            get { return m_Assembly; }
            set { m_Assembly = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region BaseType

        private String m_BaseType;

        [Optional]
        [Description("(Optional) base class name and/or interfance(s).")]
        public String BaseType {
            get { return m_BaseType; }
            set { m_BaseType = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region IsPartial

        private const bool m_DefaultIsPartial = true;

        private bool m_IsPartial = m_DefaultIsPartial;

        [Optional]
        [DefaultValue(m_DefaultIsPartial)]
        [Description("(Optional) If to generate a partial class/interface.")]
        public bool IsPartial {
            get { return m_IsPartial; }
            set { m_IsPartial = value; }
        }

        #endregion

        #region Usings

        private StringCollection m_Usings = new StringCollection();

        [Optional]
        [Description("Additional namespaces should be imported")]
        public StringCollection Usings {
            get { return m_Usings; }
            set { m_Usings = value; }
        }

        #endregion

    }

    #endregion

    #region EntityOption

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public class EntityOption : ClassOption {

        public EntityOption() { }

        private const String m_DefaultNameFmt = "{0}";

        private String m_NameFmt = m_DefaultNameFmt;

        [DefaultValue(m_DefaultNameFmt)]
        [Description("The entity class name. The first format arg is the result converted from table name.")]
        public String NameFmt {
            get { return m_NameFmt; }
            set { m_NameFmt = CSHelper.Trim(value, true); }
        }

        #region PrefixToTrim

        private String m_PrefixToTrim;

        [Optional]
        [Description("(Optional) The table prefix that will be stripped from the class name.")]
        public String PrefixToTrim {
            get { return m_PrefixToTrim; }
            set { m_PrefixToTrim = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region SuffixToTrim

        private String m_SuffixToTrim;

        [Optional]
        [Description("(Optional) The table suffix that will be stripped from the class name.")]
        public String SuffixToTrim {
            get { return m_SuffixToTrim; }
            set { m_SuffixToTrim = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region InPascalCase

        private const bool m_DefaultInPascalCase = true;

        private bool m_InPascalCase = m_DefaultInPascalCase;

        [DefaultValue(m_DefaultInPascalCase)]
        [Description("Ensures the class name is in pascal case.")]
        public bool InPascalCase {
            get { return m_InPascalCase; }
            set { m_InPascalCase = value; }
        }

        #endregion

        #region InSingular

        private const bool m_DefaultInSingular = false;

        private bool m_InSingular = m_DefaultInSingular;

        [DefaultValue(m_DefaultInSingular)]
        [Description("Removes the last 's' for ensuring the class name is in singular form.")]
        public bool InSingular {
            get { return m_InSingular; }
            set { m_InSingular = value; }
        }

        #endregion

    }

    #endregion

    #region MemberOption

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public class MemberOption {

        #region PrefixToTrim

        private String m_PrefixToTrim;

        [Optional]
        [Description("(Optional) The column prefix that will be stripped from the property name.")]
        public String PrefixToTrim {
            get { return m_PrefixToTrim; }
            set { m_PrefixToTrim = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region SuffixToTrim

        private String m_SuffixToTrim;

        [Optional]
        [Description("(Optional) The column Suffix that will be stripped from the property name.")]
        public String SuffixToTrim {
            get { return m_SuffixToTrim; }
            set { m_SuffixToTrim = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region TrimTablePrefix

        private const bool m_DefaultTrimTablePrefix = true;

        private bool m_TrimTablePrefix = m_DefaultTrimTablePrefix;

        [DefaultValue(m_DefaultTrimTablePrefix)]
        [Description("Checks if the column is prefixed with the table name and strips.")]
        public bool TrimTablePrefix {
            get { return m_TrimTablePrefix; }
            set { m_TrimTablePrefix = value; }
        }

        #endregion

        #region TrimTableSuffix

        private const bool m_DefaultTrimTableSuffix = true;

        private bool m_TrimTableSuffix = m_DefaultTrimTableSuffix;

        [DefaultValue(m_DefaultTrimTableSuffix)]
        [Description("Checks if the column is prefixed with the class name and strips.")]
        public bool TrimTableSuffix {
            get { return m_TrimTableSuffix; }
            set { m_TrimTableSuffix = value; }
        }

        #endregion

        #region TablePrePrefix

        private String m_TablePrePrefix;

        [Description("(Optional) specifies the table prefix that will be trimmed before applying TrimTablePrefix.")]
        public String TablePrePrefix {
            get { return m_TablePrePrefix; }
            set { m_TablePrePrefix = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region TablePostSuffix

        private String m_TablePostSuffix;

        [Description("(Optional) specifies the table suffix that will be trimmed before applying TrimTableSuffix.")]
        public String TablePostSuffix {
            get { return m_TablePostSuffix; }
            set { m_TablePostSuffix = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region PropInPascalCase

        private const bool m_DefaultPropInPascalCase = true;

        private bool m_PropInPascalCase = m_DefaultPropInPascalCase;

        [DefaultValue(m_DefaultPropInPascalCase)]
        [Description("Ensures the property name in pascal case.")]
        public bool PropInPascalCase {
            get { return m_PropInPascalCase; }
            set { m_PropInPascalCase = value; }
        }

        #endregion

        #region PropAccess

        private const MemberAccess M_DefaultPropAccess = MemberAccess.Public;

        private MemberAccess m_PropAccess = M_DefaultPropAccess;

        [DefaultValue(M_DefaultPropAccess)]
        [Description("Gets or sets the visibility of property.")]
        public MemberAccess PropAccess {
            get { return m_PropAccess; }
            set { m_PropAccess = value; }
        }

        #endregion

        #region VarInCamelCase

        private const bool m_DefaultVarInCamelCase = true;

        private bool m_VarInCamelCase = m_DefaultVarInCamelCase;

        [DefaultValue(m_DefaultVarInCamelCase)]
        [Description("Ensures the member variable name in camel case.")]
        public bool VarInCamelCase {
            get { return m_VarInCamelCase; }
            set { m_VarInCamelCase = value; }
        }

        #endregion

        #region VarPrefix

        private String m_VarPrefix = "m_";

        [Optional]
        [DefaultValue("m_")]
        [Description("(Optional) The prefix of the member variable name.")]
        public String VarPrefix {
            get { return m_VarPrefix; }
            set { m_VarPrefix = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region VarAccess

        private const MemberAccess m_DefaultVarAccess = MemberAccess.Private;

        private MemberAccess m_VarAccess = m_DefaultVarAccess;

        [DefaultValue(m_DefaultVarAccess)]
        [Description("The visibility of member variable.")]
        public MemberAccess VarAccess {
            get { return m_VarAccess; }
            set { m_VarAccess = value; }
        }

        #endregion

        #region UseNullable

        private const bool m_DefaultUseNullable = true;

        private bool m_UseNullable = m_DefaultUseNullable;

        [DefaultValue(m_DefaultUseNullable)]
        [Description("Indicates if System.Nullable{T} will be used for columns allow null.")]
        public bool UseNullable {
            get { return m_UseNullable; }
            set { m_UseNullable = value; }
        }

        #endregion

    }

    #endregion

    #region DaoIntfOption

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public class DaoIntfOption : ClassOption {

        private const String m_DefaultNameFmt = "I{0}Dao";

        private String m_NameFmt = m_DefaultNameFmt;

        [DefaultValue(m_DefaultNameFmt)]
        [Description("The DAO interface name. The first format arg is the Entity class name")]
        public String NameFmt {
            get { return m_NameFmt; }
            set { m_NameFmt = CSHelper.Trim(value, true); }
        }

    }

    #endregion

    #region DaoImplOption

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public class DaoImplOption : ClassOption {

        private const String m_DefaultNameFmt = "{0}Dao";

        private String m_NameFmt = m_DefaultNameFmt;

        [DefaultValue(m_DefaultNameFmt)]
        [Description("The DAO implementation class name. The first format arg is the Entity class name")]
        public String NameFmt {
            get { return m_NameFmt; }
            set { m_NameFmt = CSHelper.Trim(value, true); }
        }

        private const String m_DefaultSqlMapperFmt = "SqlMapper.Instance()";

        private String m_SqlMapperFmt = m_DefaultSqlMapperFmt;

        [DefaultValue(m_DefaultSqlMapperFmt)]
        [Description("Customize the way to get the ISqlMapper instance.")]
        public String SqlMapperFmt {
            get { return m_SqlMapperFmt; }
            set { m_SqlMapperFmt = value; }
        }
    }

    #endregion

    #region DaoMethodOption

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public class DaoMethodOption {

        /// <summary>This constructor used in XML serialization only. 
        /// It's marked as <see cref="ObsoleteAttribute"/> for preventing unexpected calls.</summary>
        [Obsolete]
        public DaoMethodOption() { }

        public DaoMethodOption(String stmtIdFmt) {
            if (String.IsNullOrEmpty(stmtIdFmt)) throw new ArgumentNullException("stmtIdFmt");
            m_IdFmt = stmtIdFmt;
            m_DefaultStmtIdFmt = stmtIdFmt;
        }

        #region GenerateIntf

        private const bool m_DefaultGenerateIntf = true;

        private bool m_GenerateIntf = m_DefaultGenerateIntf;

        [DefaultValue(m_DefaultGenerateIntf)]
        [Description("Indicates if to generate the method in DAO interface.")]
        public bool GenerateIntf {
            get { return m_GenerateIntf; }
            set { m_GenerateIntf = value; }
        }

        #endregion

        #region GenerateImpl

        private const bool m_DefaultGenerateImpl = true;

        private bool m_GenerateImpl = m_DefaultGenerateImpl;

        [DefaultValue(m_DefaultGenerateImpl)]
        [Description("Indicates if to generate the method in DAO implementation class.")]
        public bool GenerateImpl {
            get { return m_GenerateImpl; }
            set { m_GenerateImpl = value; }
        }

        #endregion

        #region IdFmt

        private String m_IdFmt;

        [Description("The format string of SQL statement ID. {0} is IdBase, {1} is PropName.")]
        public String IdFmt {
            get { return m_IdFmt; }
            set {
                if (String.IsNullOrEmpty(value) || String.IsNullOrEmpty(value.Trim())) throw new ArgumentNullException("value");
                m_IdFmt = value.Trim();
            }
        }

        private String m_DefaultStmtIdFmt;

        private bool ShouldSerializeStmtIdFmt() {
            return IdFmt != m_DefaultStmtIdFmt;
        }

        #endregion

    }

    #endregion

    #region DaoMethodSetOption

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public class DaoMethodSetOption {

        #region GetCountOption

        private DaoMethodOption m_GetCountOption = new DaoMethodOption("{0}GetCount");

        [Description("Gets or sets the options for GetCount method.")]
        public DaoMethodOption GetCountOption {
            get { return m_GetCountOption; }
            set { m_GetCountOption = value; }
        }

        #endregion

        #region FindOption

        private DaoMethodOption m_FindOption = new DaoMethodOption("{0}Find");

        [Description("Gets or sets the options for Find by primary key method.")]
        public DaoMethodOption FindOption {
            get { return m_FindOption; }
            set { m_FindOption = value; }
        }

        #endregion

        #region FindNonLobOption

        private DaoMethodOption m_FindNonLobOption = new DaoMethodOption("{0}QuickFind");

        [Description("Gets or sets the options for FindNonLob method.")]
        public DaoMethodOption FindNonLobOption {
            get { return m_FindNonLobOption; }
            set { m_FindNonLobOption = value; }
        }

        #endregion

        #region FindAllOption

        private DaoMethodOption m_FindAllOption = new DaoMethodOption("{0}FindAll");

        [Description("Gets or sets the options for FindAll method.")]
        public DaoMethodOption FindAllOption {
            get { return m_FindAllOption; }
            set { m_FindAllOption = value; }
        }

        #endregion

        #region FindNonLobAllOption

        private DaoMethodOption m_FindNonLobAllOption = new DaoMethodOption("{0}QuickFindAll");

        [Description("Gets or sets the options for FindNonLobAll method.")]
        public DaoMethodOption FindNonLobAllOption {
            get { return m_FindNonLobAllOption; }
            set { m_FindNonLobAllOption = value; }
        }

        #endregion

        #region FindByOption

        private DaoMethodOption m_FindByOption = new DaoMethodOption("{0}FindBy{1}");

        [Description("Gets or sets the options for FindBy method.")]
        public DaoMethodOption FindByOption {
            get { return m_FindByOption; }
            set { m_FindByOption = value; }
        }

        #endregion

        #region FindNonLobByOption

        private DaoMethodOption m_FindNonLobByOption = new DaoMethodOption("{0}QuickFindBy{1}");

        [Description("Gets or sets the options for FindNonLobBy method.")]
        public DaoMethodOption FindNonLobByOption {
            get { return m_FindNonLobByOption; }
            set { m_FindNonLobByOption = value; }
        }

        #endregion

        #region InsertOption

        private DaoMethodOption m_InsertOption = new DaoMethodOption("{0}Insert");

        [Description("Gets or sets the options for Insert method.")]
        public DaoMethodOption InsertOption {
            get { return m_InsertOption; }
            set { m_InsertOption = value; }
        }

        #endregion

        #region UpdateOption

        private DaoMethodOption m_UpdateOption = new DaoMethodOption("{0}Update");

        [Description("Gets or sets the options for Update method.")]
        public DaoMethodOption UpdateOption {
            get { return m_UpdateOption; }
            set { m_UpdateOption = value; }
        }

        #endregion

        #region DeleteOption

        private DaoMethodOption m_DeleteOption = new DaoMethodOption("{0}Delete");

        [Description("Gets or sets the options for Delete method.")]
        public DaoMethodOption DeleteOption {
            get { return m_DeleteOption; }
            set { m_DeleteOption = value; }
        }

        #endregion

        #region DeleteByOption

        private DaoMethodOption m_DeleteByOption = new DaoMethodOption("{0}DeleteBy{1}");

        [Description("Gets or sets the options for DeleteBy method.")]
        public DaoMethodOption DeleteByOption {
            get { return m_DeleteByOption; }
            set { m_DeleteByOption = value; }
        }

        #endregion

        #region ReloadOption

        private DaoMethodOption m_ReloadOption = new DaoMethodOption("{0}Reload");

        [Description("Gets or sets the options for Reload method.")]
        public DaoMethodOption ReloadOption {
            get { return m_ReloadOption; }
            set { m_ReloadOption = value; }
        }

        #endregion
    
    }

    #endregion

    #region SqlOption

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public class SqlOption {

        #region VendorId

        private const DbVendorId m_DefaultVendorId = DbVendorId.SQLServer;

        private DbVendorId m_VendorId = m_DefaultVendorId;

        [DefaultValue(m_DefaultVendorId)]
        [Description("Gets or sets the database vendor.")]
        public DbVendorId VendorId {
            get { return m_VendorId; }
            set { m_VendorId = value; }
        }

        #endregion

        #region Statement Ids

        #region IdPattern

        private const StmtIdPattern m_DefaultIdPattern = StmtIdPattern.Local;

        private StmtIdPattern m_IdPattern = m_DefaultIdPattern;

        [Optional]
        [DefaultValue(m_DefaultIdPattern)]
        [Description("Controls how the statement name is generated.")]
        public StmtIdPattern IdPattern {
            get { return m_IdPattern; }
            set { m_IdPattern = value; }
        }

        #endregion

        #region IdDelimiter

        private const String m_DefaultIdDelimiter = "-";

        private String m_IdDelimiter = m_DefaultIdDelimiter;

        [Optional]
        [DefaultValue(m_DefaultIdDelimiter)]
        [Description("(Optional) The delimiter for different parts of a statement name. The default is '-'")]
        public String IdDelimiter {
            get { return m_IdDelimiter; }
            set { m_IdDelimiter = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region FullResultMapIdFmt

        private const String m_DefaultFullResultMapIdFmt = "{0}FullResultMap";

        private String m_FullResultMapIdFmt = m_DefaultFullResultMapIdFmt;

        [DefaultValue(m_DefaultFullResultMapIdFmt)]
        [Description("The format for the Id the result map that contains all columns.")]
        public String FullResultMapIdFmt {
            get { return m_FullResultMapIdFmt; }
            set { m_FullResultMapIdFmt = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #region NonLobResultMapIdFmt

        private const String m_DefaultNonLobResultMapIdFmt = "{0}NonLobResultMap";

        private String m_NonLobResultMapIdFmt = m_DefaultNonLobResultMapIdFmt;

        [DefaultValue(m_DefaultNonLobResultMapIdFmt)]
        [Description("The format for the Id the result map that contains all columns except Lobs.")]
        public String NonLobResultMapIdFmt {
            get { return m_NonLobResultMapIdFmt; }
            set { m_NonLobResultMapIdFmt = (value != null) ? value.Trim() : null; }
        }

        #endregion

        #endregion

        #region TableQualified

        private const bool m_DefaultTableQualified = false;

        private bool m_TableQualified = m_DefaultTableQualified;

        [DefaultValue(m_DefaultTableQualified)]
        [Description("If the table name is qualified, such as dbo.sysobjects for sysobjects.")]
        public bool TableQualified {
            get { return m_TableQualified; }
            set { m_TableQualified = value; }
        }

        #endregion

        #region IncludeDbType

        private bool m_IncludeDbType;

        /// <summary>Indicates if the generated inline parameter map includes the dbType attribute.</summary>
        [Description("Indiciates if to include \"dbType\" in inline parameter maps")]
        public bool IncludeDbType {
            get { return m_IncludeDbType; }
            set { m_IncludeDbType = value; }
        }

        #endregion

    }

    #endregion

    #region FileOption

    /// <summary><c>FileOption</c> contains options for a type of generated file.</summary>
    /// <remarks><see cref="Dir"/> and <see cref="FileNameFmt"/> converts <c>null</c> to <see cref="String.Empty"/>
    /// for simplifying the XML serialization.</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public class FileOption : IXmlSerializable {
        
        /// <summary>This constructor used in XML serialization only. 
        /// It's marked as <see cref="ObsoleteAttribute"/> for preventing unexpected calls.</summary>
        [Obsolete]
        public FileOption() {
            m_Dir = String.Empty;
            m_FileNameFmt = String.Empty;
        }
        
        public FileOption(bool save, String dir, String fileNameFmt) {
            m_Save = save;
            m_Dir = (dir != null) ? dir.Trim() : String.Empty;
            m_FileNameFmt = (fileNameFmt != null) ? fileNameFmt.Trim() : String.Empty;

            m_DefaultSave = m_Save;
            m_DefaultDir = m_Dir;
            m_DefaultFileNameFmt = m_FileNameFmt;
        }

        #region Save

        private bool m_Save;

        /// <summary>Indicates if to save the IBatisNet mapping to a file.</summary>
        [Description("Indicates if to save the IBatisNet mapping to a file.")]
        public bool Save {
            get { return m_Save; }
            set { m_Save = value; }
        }
        
        private bool m_DefaultSave = true;

        private bool ShouldSerializeSave() {
            return Save != m_DefaultSave;
        }

        #endregion

        #region Dir

        private String m_Dir;

        /// <summary>Gets or sets the directory to put the generated file.</summary>
        /// <remarks>The default is "Output\s" under the directory where the template lives.</remarks>
        [Optional]
        [Description("Gets or sets the directory to put the generated IBatisNet mapping file.")]
        public String Dir {
            get { return m_Dir; }
            set { m_Dir = (value != null) ? value.Trim() : String.Empty; }
        }
        
        private String m_DefaultDir;

        private bool ShouldSerializeDir() {
            return Dir != m_DefaultDir;
        }
        
        #endregion

        #region FileNameFmt

        private String m_FileNameFmt;

        /// <summary>Gets or sets the name format of the generated IBatisNet mapping file.</summary>
        /// <remarks>The default is "{0}..xml". 
        /// The first argument will be replace with the class name during generation.</remarks>
        [Optional]
        [Description("Gets or sets the name of the generated IBatisNet mapping file.")]
        public String FileNameFmt {
            get { return m_FileNameFmt; }
            set { m_FileNameFmt = (value != null) ? value.Trim() : String.Empty; }
        }
        
        private String m_DefaultFileNameFmt = "{0}.xml";

        private bool ShouldSerializeFileNameFmt() {
            return FileNameFmt != m_DefaultFileNameFmt;
        }
        
        #endregion

        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            m_Save = Boolean.Parse(reader.GetAttribute("Save"));
            m_DefaultSave = Boolean.Parse(reader.GetAttribute("DefaultSave"));
            m_Dir = reader.GetAttribute("Dir");
            m_DefaultDir = reader.GetAttribute("DefaultDir");
            m_FileNameFmt = reader.GetAttribute("FileNameFmt");
            m_DefaultFileNameFmt = reader.GetAttribute("DefaultFileNameFmt");
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("Save", Save.ToString());
            writer.WriteAttributeString("DefaultSave", m_DefaultSave.ToString());
            writer.WriteAttributeString("Dir", (Dir != null) ? Dir : String.Empty);
            writer.WriteAttributeString("DefaultDir", m_DefaultDir);
            writer.WriteAttributeString("FileNameFmt", FileNameFmt);
            writer.WriteAttributeString("DefaultFileNameFmt", m_DefaultFileNameFmt);
        }

        #endregion

    }

    #endregion

    #region FileSetOption

    /// <summary><c>FileSetOption</c> contains a group of <see cref="FileOption"/> for each file to be generated.</summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [PropertySerializer(typeof(XmlPropertySerializer))]
    public class FileSetOption {

        public FileSetOption() { }

        #region BatisMapOption

        public static FileOption CreateDefaultBatisMapOption() {
            return new FileOption(false, @"Output\BatisMap", "{0}.BatisMap.xml");
        }

        private FileOption m_BatisMapOption = CreateDefaultBatisMapOption();

        [Description("The file options for generated IBatisNet mapping")]
        public FileOption BatisMapOption {
            get { return m_BatisMapOption; }
            set { m_BatisMapOption = value; }
        }

        #endregion

        #region EntityOption

        public static FileOption CreateDefaultEntityOption() {
            return new FileOption(false, @"Output\Entity", "{0}.cs");
        }

        private FileOption m_EntityOption = CreateDefaultEntityOption();

        [Description("The file options for generated data class")]
        public FileOption EntityOption {
            get { return m_EntityOption; }
            set { m_EntityOption = value; }
        }

        #endregion

        #region DaoIntfOption

        public static FileOption CreateDefaultDaoIntfOption() {
            return new FileOption(false, @"Output\DaoIntf", "I{0}Dao.cs");
        }

        private FileOption m_DaoIntfOption = CreateDefaultDaoIntfOption();

        [Description("The file options for generated data access interfaces")]
        public FileOption DaoIntfOption {
            get { return m_DaoIntfOption; }
            set { m_DaoIntfOption = value; }
        }

        #endregion

        #region DaoImplOption

        public static FileOption CreateDefaultDaoImplOption() {
            return new FileOption(false, @"Output\DaoImpl", "{0}Dal.cs");
        }

        private FileOption m_DaoImplOption = CreateDefaultDaoImplOption();

        [Description("The file options for generated data access implementations")]
        public FileOption DaoImplOption {
            get { return m_DaoImplOption; }
            set { m_DaoImplOption = value; }
        }

        #endregion

    }

    #endregion

    #region IDbVendor

    /// <summary><c>IDbVender</c> collects database vendor specific utilities.</summary>
    /// <remarks>Implementor classes should have a public parameterless constructor.</remarks>
    public interface IDbVendor {

        /// <summary>Gets the CLR type of a column.</summary>
        /// <param name="column">The column.</param>
        /// <param name="useNullable">if Nullable{T} can be used for columns allow database NULL.</param>
        Type GetClrType(ColumnSchema column, bool useNullable);

        /// <summary>Determinates if a column is LOB (BLOB, CLOB, IMAGE, TEXT, etc).</summary>
        bool IsLob(ColumnSchema column);

        /// <summary>Gets the quoted string.</summary>
        String QuotedStr(String s);

        /// <summary>Gets the table declaraction used in SQL statement.</summary>
        String GetTableDecl(TableSchema table);

        /// <summary>Gets an object that identifies the ADO.NET provider specific data type, such as a <see cref="SqlDbType"/>.</summary>
        Object GetProviderDbType(ColumnSchema column);

        /// <summary>Gets the string representation of <see cref="GetProviderDbType"/>.</summary>
        String GetProviderDbTypeName(Object providerDbType);

        /// <summary>Indicates if the provider specific dbType required for IBatis.NET's inline parameter map.</summary>
        bool IsExplicit(ColumnSchema column);

    }

    #endregion

    #region MSSqlDbVendor

    /// <summary><c>MSSqlDbVendor</c> implements <see cref="IDbVendor"/> for Microsoft SQL Server.</summary>
    public class MSSqlDbVendor : IDbVendor {

        #region GetClrType

        /// <summary>Implements <see cref="IDbVendor.GetClrType"/>.</summary>
        public Type GetClrType(ColumnSchema column, bool useNullable) {
            if (column == null) throw new ArgumentNullException("column");

            SqlDbType sqlt = GetSqlDbType(column);
            Type result = GetClrPrimitiveType(sqlt);

            if (useNullable == false)
                return result;
            if (column.AllowDBNull == false)
                return result;
            if (IsLob(column))
                return result;
            if (column.IsPrimaryKeyMember)
                return result;
            if (result.IsValueType == false || result.HasElementType)
                return result;

            Type genericNullable = typeof(Nullable<>);
            result = genericNullable.MakeGenericType(result);
            return result;
        }

        #endregion

        #region IsLob

        private const int m_SizeLimit = 4000;    //TODO: MSSql7, MSSql2000, MSSql2005 may vary.
        
        private readonly String[] knownLobNativeTypes = new String[] {"image", "text"};

        /// <summary>Implements <see cref="IDbVendor.IsLob"/>.</summary>
        public bool IsLob(ColumnSchema column) {
            if (column == null) throw new ArgumentNullException("column");
            
            if (Array.IndexOf<String>(knownLobNativeTypes, column.NativeType) > -1) return true;

            bool isTooLong = (column.Size < 0) || (m_SizeLimit < column.Size);
            switch (column.DataType) {
                case DbType.AnsiString: return isTooLong;
                case DbType.AnsiStringFixedLength: return isTooLong;
                case DbType.Binary: return isTooLong;
                case DbType.Boolean: return false;
                case DbType.Byte: return false;
                case DbType.Currency: return false;
                case DbType.Date: return false;
                case DbType.DateTime: return false;
                case DbType.Decimal: return false;
                case DbType.Double: return false;
                case DbType.Guid: return false;
                case DbType.Int16: return false;
                case DbType.Int32: return false;
                case DbType.Int64: return false;
                case DbType.Object: return false;
                case DbType.SByte: return false;
                case DbType.Single: return false;
                case DbType.String: return isTooLong;
                case DbType.StringFixedLength: return isTooLong;
                case DbType.Time: return false;
                case DbType.UInt16: return false;
                case DbType.UInt32: return false;
                case DbType.UInt64: return false;
                case DbType.VarNumeric: return false;
                case DbType.Xml: return true;
                default:
                    throw new NotImplementedException("Unknown DbType : " + column.DataType.ToString());
            }
        }

        #endregion

        #region QuotedStr

        public String QuotedStr(String s) { return "[" + s + "]"; }

        #endregion

        #region GetTableDecl

        public String GetTableDecl(TableSchema table) {
            String result = QuotedStr(table.Name);
            if (!String.IsNullOrEmpty(table.Owner))
                result = QuotedStr(table.Owner) + "." + result;
            return result;
        }

        #endregion

        #region GetProviderDbType

        public Object GetProviderDbType(ColumnSchema column) {
            return GetSqlDbType(column);
        }

        public String GetProviderDbTypeName(Object providerDbType) {
            if (providerDbType == null) throw new ArgumentNullException("providerdDbType");
            if (providerDbType.GetType() != typeof(SqlDbType))
                throw new ArgumentException(String.Format(
                    "Invalid type of providerDbType, expecting {0} but actually {1} !",
                    typeof(SqlDbType).AssemblyQualifiedName, providerDbType.GetType().AssemblyQualifiedName));
            return ((SqlDbType)providerDbType).ToString();
        }

        /// <summary>Implements <see cref="IDbVendor.IsExplicit"/></summary>
        public bool IsExplicit(ColumnSchema column) {
            return true;
        }

        #endregion

        #region GetSqlDbType

        /// <summary>A dictionary of SqlDbType keyed by the <see cref="String.ToLower()"/> format of <see cref="ColumnSchema.NativeType"/>.</summary>
        private Dictionary<String, SqlDbType> m_NativeTypeMap = CreateDefaultSqlDbTypeMap();

        private static Dictionary<String, SqlDbType> CreateDefaultSqlDbTypeMap() {
            Dictionary<String, SqlDbType> result = new Dictionary<String, SqlDbType>();
            String[] names = Enum.GetNames(typeof(SqlDbType));
            foreach (String name in names) {
                SqlDbType sqlt = (SqlDbType)Enum.Parse(typeof(SqlDbType), name);
                result.Add(name.ToLower(), sqlt);
            }
            return result;
        }

        /// <summary>Gets the <see cref="SqlDbType"/> of a <see cref="ColumnSchema"/>.</summary>
        private SqlDbType GetSqlDbType(ColumnSchema column) {
            if (column == null) throw new ArgumentNullException("column");

            SqlDbType sqlt;
            if (m_NativeTypeMap.TryGetValue(column.NativeType, out sqlt))
                return sqlt;

            switch (column.DataType) {
                case DbType.AnsiString: return SqlDbType.VarChar;
                case DbType.AnsiStringFixedLength: return SqlDbType.Char;
                case DbType.Binary: return SqlDbType.VarBinary;
                case DbType.Boolean: return SqlDbType.Bit;
                case DbType.Byte: return SqlDbType.TinyInt;
                case DbType.Currency: return SqlDbType.Money;
                case DbType.Date: return SqlDbType.DateTime;
                case DbType.DateTime: return SqlDbType.DateTime;
                case DbType.Decimal: return SqlDbType.Decimal;
                case DbType.Double: return SqlDbType.Float;
                case DbType.Guid: return SqlDbType.UniqueIdentifier;
                case DbType.Int16: return SqlDbType.SmallInt;
                case DbType.Int32: return SqlDbType.Int;
                case DbType.Int64: return SqlDbType.BigInt;
                //case DbType.Object: return SqlDbType;
                //case DbType.SByte: return SqlDbType;
                case DbType.Single: return SqlDbType.Real;
                case DbType.String: return SqlDbType.NVarChar;
                case DbType.StringFixedLength: return SqlDbType.NChar;
                //case DbType.Time: return SqlDbType.DateTime;
                //case DbType.UInt16: return SqlDbType;
                //case DbType.UInt32: return SqlDbType;
                //case DbType.UInt64: return SqlDbType;
                //case DbType.VarNumeric: return SqlDbType.Decimal;
                case DbType.Xml: return SqlDbType.Xml;
                default :
                    throw new NotImplementedException(String.Format(
                        "Cannot get the SqlDbType for Column of DataType {0}.",
                        column.DataType.ToString()));
            }
        }

        /// <summary>Gets the primitive CLR type.</summary>
        private static Type GetClrPrimitiveType(SqlDbType t) {
            switch (t) {
                case SqlDbType.BigInt: return typeof(Int64);
                case SqlDbType.Binary: return typeof(Byte[]);
                case SqlDbType.Bit: return typeof(Boolean);
                case SqlDbType.Char: return typeof(String);
                case SqlDbType.DateTime: return typeof(DateTime);
                case SqlDbType.Decimal: return typeof(Decimal);
                case SqlDbType.Float: return typeof(Double);
                case SqlDbType.Image: return typeof(Byte[]);
                case SqlDbType.Int: return typeof(Int32);
                case SqlDbType.Money: return typeof(Decimal);
                case SqlDbType.NChar: return typeof(String);
                case SqlDbType.NText: return typeof(String);
                case SqlDbType.NVarChar: return typeof(String);
                case SqlDbType.Real: return typeof(Single);
                case SqlDbType.SmallDateTime: return typeof(DateTime);
                case SqlDbType.SmallInt: return typeof(Int16);
                case SqlDbType.SmallMoney: return typeof(Decimal);
                case SqlDbType.Text: return typeof(String);
                case SqlDbType.Timestamp: return typeof(Byte[]);
                case SqlDbType.TinyInt: return typeof(Byte);
                case SqlDbType.Udt: return typeof(Object);
                case SqlDbType.UniqueIdentifier: return typeof(Guid);
                case SqlDbType.VarBinary: return typeof(Byte[]);
                case SqlDbType.VarChar: return typeof(String);
                case SqlDbType.Variant: return typeof(Object);
                case SqlDbType.Xml: return typeof(String);
                default:
                    throw new NotImplementedException(String.Format(
                        "Cannot get the CLR type of SqlDbType {0}",
                        t.ToString()));
            }
        }

        #endregion

    }

    #endregion

    #region DbVendorProvider

    /// <summary><c>DbVendorProvider</c> provides a common way for get <see cref="IDbVendor"/>s.</summary>
    public static class DbVendorProvider {

        private static readonly MSSqlDbVendor m_MSSqlDbVendor = new MSSqlDbVendor();

        /// <summary>Gets an instance of <see cref="IDbVendor"/> identified by <see cref="DbVendorId"/> enum value.</summary>
        public static IDbVendor GetVendor(DbVendorId vendorId) {
            switch (vendorId) {
                case DbVendorId.SQLServer: return m_MSSqlDbVendor;
                default:
                    throw new NotImplementedException("Unknown DbVendor : " + vendorId.ToString());
            }
        }

    }

    #endregion

    #region ColumnInfo

    /// <summary><c>ColumnInfo</c> build template snippets for a <see cref="ColumnSchema"/>.</summary>
    /// <remarks><c>ColumnBuilder</c> should be subclassed per database vender. <c>ColumnBuilder</c>
    /// will be instantiated by <see cref="TableBuilder"/> for each <see cref="ColumnSchema"/> instance.</remarks>
    public class ColumnInfo {

        public ColumnInfo(ColumnSchema column, MemberOption memberOption, IDbVendor dbVendor) {
            if (column == null) throw new ArgumentNullException("column");
            if (memberOption == null) throw new ArgumentNullException("memberOption");
            if (dbVendor == null) throw new ArgumentNullException("dbVendor");

            //Assign parameters to member variables.
            m_Column = column;
            m_MemberOption = memberOption;
            m_DbVendor = dbVendor;

            m_ClrType = DbVendor.GetClrType(column, MemberOption.UseNullable);
            m_ClrPrimitiveType = Nullable.GetUnderlyingType(m_ClrType);
            m_ClrPrimitiveType = (m_ClrPrimitiveType != null) ? m_ClrPrimitiveType : m_ClrType;

            m_IsClrNullable = Nullable.GetUnderlyingType(m_ClrType) != null;
            m_ClrTypeDecl = GetClrTypeDecl(m_ClrType);
            m_IsLob = DbVendor.IsLob(Column);

            m_PropName = GetPropName(Column, MemberOption);
            m_VarName = GetVarName(PropName, MemberOption);
            m_ClrParamName = CSHelper.GetCamelCase(PropName);
            m_ClrParamDecl = ClrTypeDecl + " " + ClrParamName;
            m_InitialValueDecl = m_IsClrNullable ? " = null" : String.Empty;

            m_PropAccessDecl = CSHelper.GetMemberAccessName(MemberOption.PropAccess);
            m_VarAccessDecl = CSHelper.GetMemberAccessName(MemberOption.VarAccess);

            m_ProviderDbType = DbVendor.GetProviderDbType(Column);
            m_ProviderDbTypeName = DbVendor.GetProviderDbTypeName(m_ProviderDbType);
            m_IsExpilicit = DbVendor.IsExplicit(Column);

            m_ResultColumnName = Column.Name;
            m_SqlColumnName = DbVendor.QuotedStr(Column.Name);
            m_SqlQualifiedColumnName = DbVendor.QuotedStr(column.Table.Name) + "." + m_SqlColumnName;
            m_SqlInlineParameterMap = GetSqlInlineParameterMap(PropName, ClrTypeDecl, ProviderDbTypeName, IsExpilicit);
            m_SqlParameterClass = m_ClrPrimitiveType.Name;
        }

        #region MemberOption

        private readonly MemberOption m_MemberOption;

        /// <summary>Gets the <see cref="MemberOption"/>.</summary>
        protected MemberOption MemberOption { get { return m_MemberOption; } }

        #endregion

        #region Column

        private readonly ColumnSchema m_Column;

        protected ColumnSchema Column {
            get { return m_Column; }
        }

        #endregion

        #region DbVendor

        private readonly IDbVendor m_DbVendor;

        protected IDbVendor DbVendor {
            get { return m_DbVendor; }
        }

        #endregion

        #region ClrPrimitiveType

        private readonly Type m_ClrPrimitiveType;

        protected Type ClrPrimitiveType {
            get { return m_ClrPrimitiveType; }
        }

        #endregion

        #region ClrType

        private readonly Type m_ClrType;

        /// <summary>The CLR type for declaring property/variable/parameter.</summary>
        protected Type ClrType { get { return m_ClrType; } }

        #endregion

        #region IsClrNullable

        private readonly bool m_IsClrNullable;

        /// <summary>Indicates if the <see cref="ClrType"/> is a <see cref="Nullable"/> type.</summary>
        protected bool IsClrNullable { get { return m_IsClrNullable; } }

        #endregion

        #region ClrTypeDecl

        private readonly String m_ClrTypeDecl;

        /// <summary>Gets the string representation of the CLR type definition.</summary>
        /// <remarks>The string is in C# syntax.</remarks>
        /// <example>"int", "byte", "byte[]", "int?"</example>
        public String ClrTypeDecl { get { return m_ClrTypeDecl; } }

        #endregion

        #region IsLob

        private readonly bool m_IsLob;

        /// <summary>Indicates if this column is a LOB column.</summary>
        public bool IsLob { get { return m_IsLob; } }

        #endregion

        #region PropName

        private readonly String m_PropName;

        /// <summary>Gets the name of the entity property.</summary>
        public String PropName { get { return m_PropName; } }

        #endregion

        #region VarName

        private readonly String m_VarName;

        /// <summary>Gets the name of the entity variable.</summary>
        public String VarName { get { return m_VarName; } }

        #endregion

        #region ClrParamName

        private readonly String m_ClrParamName;

        /// <summary>Gets name of the CLR parameter.</summary>
        public String ClrParamName { get { return m_ClrParamName; } }

        #endregion

        #region ClrParamDecl

        private readonly String m_ClrParamDecl;

        /// <summary>Gets the declaraction of the CLR parameter.</summary>
        /// <value>A string like "int id", "Byte[] stm".</value>
        public String ClrParamDecl { get { return m_ClrParamDecl; } }

        #endregion

        #region InitialValueDecl

        private readonly String m_InitialValueDecl;

        /// <summary>Gets the initial value declaraction.</summary>
        public String InitialValueDecl { get { return m_InitialValueDecl; } }

        #endregion

        #region PropAccessDecl

        private readonly String m_PropAccessDecl;

        /// <summary>Gets the string representation for the access modifier of the entity property declaraction.</summary>
        public String PropAccessDecl { get { return m_PropAccessDecl; } }

        #endregion

        #region VarAccessDecl

        private readonly String m_VarAccessDecl;

        /// <summary>Gets the string representation for the access modifier of the entity variable declaraction.</summary>
        public String VarAccessDecl { get { return m_VarAccessDecl; } }

        #endregion

        #region ProviderDbType

        /// <summary><see cref="SqlDbType"/> for MSSql, <c>OracleType"</c> for oracle, etc.</summary>
        private readonly Object m_ProviderDbType;

        private readonly String m_ProviderDbTypeName;

        /// <summary>Gets the name of the underlying ADO.NET provider's data type.</summary>
        /// <remarks>The result is used by the "dbType" in IBatis.NET's inline parameter map.</remarks>
        public String ProviderDbTypeName {
            get { return m_ProviderDbTypeName; }
        }

        private readonly bool m_IsExpilicit;

        /// <summary>Indicates if the provider specific dbType required for IBatis.NET's inline parameter map.</summary>
        public bool IsExpilicit {
            get { return m_IsExpilicit; }
        }

        #endregion

        #region ResultColumnName

        private readonly String m_ResultColumnName;

        /// <summary>Gets the value of the "column" attribute in "result" element in "resultMap".</summary>
        public String ResultColumnName {
            get { return m_ResultColumnName; }
        }

        #endregion

        #region SqlColumnName

        private readonly String m_SqlColumnName;

        /// <summary>Gets the expression when a column is used in SQL selection/condition clause.</summary>
        public String SqlColumnName {
            get { return m_SqlColumnName; }
        }

        #endregion

        #region SqlQualifiedColumnName

        private readonly String m_SqlQualifiedColumnName;

        public String SqlQualifiedColumnName {
            get { return m_SqlQualifiedColumnName; }
        }

        #endregion

        #region SqlInlineParameterMap

        private String m_SqlInlineParameterMap;

        public String SqlInlineParameterMap {
            get { return m_SqlInlineParameterMap; }
        }

        private static String GetSqlInlineParameterMap(String propName, String clrTypeDecl, String providerDbTypeName, bool isExplicit) {
            String s = propName;
            //s = s + ", type=" + clrTypeDecl;
            if (isExplicit && (String.IsNullOrEmpty(providerDbTypeName) == false))
                s = s + ",dbType=" + providerDbTypeName;
            return s;
        }

        #endregion

        #region SqlParameterClass

        private readonly String m_SqlParameterClass;

        public String SqlParameterClass {
            get { return m_SqlParameterClass; }
        }

        #endregion

        #region Static utility methods

        private static String GetClrTypeDecl(Type clrType) {
            Type nt = Nullable.GetUnderlyingType(clrType);
            if (nt == null)
                return clrType.Name;
            return nt.Name + "?";
        }

        private static String GetPropName(ColumnSchema column, MemberOption option) {
            String result = column.Name;

            if (CSHelper.IsNotBlank(option.PrefixToTrim)) {
                result = CSHelper.StripPrefix(result, option.PrefixToTrim);
            }

            if (option.TrimTablePrefix) {
                string prefix = column.Table.Name;
                if (CSHelper.IsNotBlank(option.TablePrePrefix)) {
                    prefix = CSHelper.StripPrefix(prefix, option.TablePrePrefix);
                }
                result = CSHelper.StripPrefix(result, prefix);
            }

            if (CSHelper.IsNotBlank(option.SuffixToTrim)) {
                result = CSHelper.StripSuffix(result, column.Table.Name);
            }

            if (option.TrimTableSuffix) {
                string suffix = column.Table.Name;
                if (CSHelper.IsNotBlank(option.TablePostSuffix)) {
                    suffix = CSHelper.StripSuffix(suffix, option.TablePostSuffix.Trim());
                }
                result = CSHelper.StripPrefix(result, suffix);
            }

            if (option.PropInPascalCase) {
                result = CSHelper.GetPascalCase(result);
            }

            result = CSHelper.GetCSharpIdentifier(result);
            return result;
        }

        private static String GetVarName(String propName, MemberOption option) {
            String result = propName;
            if (option.VarInCamelCase)
                result = CSHelper.GetCamelCase(result);
            result = option.VarPrefix + result;
            return result;
        }

        #endregion

    }

    #endregion

    #region ITableBuilderOption

    /// <summary><c>ITableBuilderOption</c> contains a group of options used by <see cref="TableBuilder"/>.</summary>
    public interface ITableBuilderOption {

        /// <summary>Gets the options for data class generation.</summary>
        EntityOption EntityOption { get; }

        /// <summary>Gets the options for property/variable generation.</summary>
        MemberOption MemberOption { get; }

        /// <summary>Gets the options for data access interface generation.</summary>
        DaoIntfOption DaoIntfOption { get; }

        /// <summary>Gets the options for data access class generation.</summary>
        DaoImplOption DaoImplOption { get; }

        /// <summary>Gets the options for data access methods.</summary>
        DaoMethodSetOption DaoMethodSetOption { get; }

        /// <summary>Gets the options for SQL generation.</summary>
        SqlOption SqlOption { get; }

    }

    #endregion

    #region TableBuilder

    /// <summary><c>Builder</c> provides sharing build result among <c>BatisBaseTemplate</c>s for the same database table.</summary>
    public class TableBuilder : ITableBuilderOption {

        /// <summary>Creates an instance.</summary>
        public TableBuilder(TableSchema sourceTable, ITableBuilderOption builderOption) {
            if (sourceTable == null) throw new ArgumentNullException("sourceTable");
            if (builderOption == null) throw new ArgumentNullException("builderOption");

            m_SourceTable = sourceTable;
            m_BuilderOption = builderOption;
            m_DbVendor = DbVendorProvider.GetVendor(m_BuilderOption.SqlOption.VendorId);

            m_TableName = DbVendor.QuotedStr(SourceTable.Name);
            m_QualifiedTableName = DbVendor.QuotedStr(SourceTable.Owner) + "." + TableName;

            m_EntityNamespace = EntityOption.Namespace;
            m_EntityName = GetEntityName(SourceTable.Name, EntityOption);
            m_EntityFullName = CSHelper.GetQualifiedName(EntityName, EntityNamespace);
            m_EntityQualifiedName = CSHelper.GetAssemblyQualifiedName(EntityFullName, EntityOption.Assembly);
            m_EntityPartialDecl = EntityOption.IsPartial ? "partial " : String.Empty;
            m_EntityBaseDecl = String.IsNullOrEmpty(EntityOption.BaseType) ? String.Empty : " : " + EntityOption.BaseType;

            m_DaoIntfNamespace = DaoIntfOption.Namespace;
            m_DaoIntfName = String.Format(DaoIntfOption.NameFmt, EntityName);
            m_DaoIntfFullName = CSHelper.GetQualifiedName(DaoIntfName, DaoIntfNamespace);
            m_DaoIntfQualifiedName = CSHelper.GetAssemblyQualifiedName(DaoIntfFullName, DaoIntfOption.Assembly);
            m_DaoIntfPartialDecl = DaoIntfOption.IsPartial ? "partial " : String.Empty;
            m_DaoIntfBaseDecl = String.IsNullOrEmpty(DaoIntfOption.BaseType) ? String.Empty : " : " + DaoIntfOption.BaseType;

            m_DaoImplNamespace = DaoImplOption.Namespace;
            m_DaoImplName = String.Format(DaoImplOption.NameFmt, EntityName);
            m_DaoImplFullName = CSHelper.GetQualifiedName(DaoImplName, DaoImplNamespace);
            m_DaoImplQualifiedName = CSHelper.GetAssemblyQualifiedName(DaoImplQualifiedName, DaoImplOption.Assembly);
            m_DaoImplPartialDecl = DaoImplOption.IsPartial ? "partial " : String.Empty;
            m_DaoImplBaseDecl = " : " + DaoImplOption.BaseType 
                + (String.IsNullOrEmpty(DaoImplOption.BaseType) ? String.Empty : ", ") 
                + m_DaoIntfName;
            m_SqlMapper = DaoImplOption.SqlMapperFmt;

            int columnCount = SourceTable.Columns.Count;
            bool isSingleColumnPk = SourceTable.PrimaryKey.MemberColumns.Count == 1;    //If the table has a single-column primary key.

            //Initialize column lists
            List<ColumnInfo> columns = new List<ColumnInfo>(columnCount);
            List<ColumnInfo> pkColumns = new List<ColumnInfo>(SourceTable.PrimaryKey.MemberColumns.Count);
            List<ColumnInfo> nonPkColumns = new List<ColumnInfo>(columnCount);
            List<ColumnInfo> finderColumns = new List<ColumnInfo>(columnCount);
            List<ColumnInfo> lobColumns = new List<ColumnInfo>(columnCount);
            List<ColumnInfo> nonLobColumns = new List<ColumnInfo>(columnCount);

            foreach (ColumnSchema c in SourceTable.Columns) {
                ColumnInfo cb = new ColumnInfo(c, MemberOption, DbVendor);
                columns.Add(cb);

                if (c.IsPrimaryKeyMember) {
                    pkColumns.Add(cb);
                } else {
                    nonPkColumns.Add(cb);
                }

                if (cb.IsLob)
                    lobColumns.Add(cb);
                else
                    nonLobColumns.Add(cb);

                //Determinates if the column will be used in FindByXXX.
                if (c.IsPrimaryKeyMember && isSingleColumnPk) continue;
                if (cb.IsLob) continue;
                finderColumns.Add(cb);
            }
            m_Columns = columns.AsReadOnly();
            m_PkColumns = pkColumns.AsReadOnly();
            m_NonPkColumns = nonPkColumns.AsReadOnly();
            m_FinderColumns = finderColumns.AsReadOnly();
            m_LobColumns = lobColumns.AsReadOnly();
            m_NonLobColumns = nonLobColumns.AsReadOnly();

            m_HasLob = lobColumns.Count > 0;
            m_IdBase = GetIdBase(m_EntityName, m_EntityNamespace, SqlOption);
        }

        #region ITableBuilderOption members

        private readonly ITableBuilderOption m_BuilderOption;

        public EntityOption EntityOption { get { return m_BuilderOption.EntityOption; } }

        public MemberOption MemberOption { get { return m_BuilderOption.MemberOption; } }

        public DaoIntfOption DaoIntfOption { get { return m_BuilderOption.DaoIntfOption; } }

        public DaoImplOption DaoImplOption { get { return m_BuilderOption.DaoImplOption; } }

        public DaoMethodSetOption DaoMethodSetOption { get { return m_BuilderOption.DaoMethodSetOption; } }

        public SqlOption SqlOption { get { return m_BuilderOption.SqlOption; } }

        #endregion

        #region DbVendor

        private readonly IDbVendor m_DbVendor;

        protected IDbVendor DbVendor { get { return m_DbVendor; } }

        #endregion

        #region SourceTable

        private readonly TableSchema m_SourceTable;

        /// <summary>Gets the source <see cref="TableSchema"/>.</summary>
        public TableSchema SourceTable {
            get { return m_SourceTable; }
        }

        #endregion

        #region TableDecl

        private readonly String m_TableName;

        /// <summary>Gets the table name used in SQL statement.</summary>
        public String TableName {
            get { return m_TableName; }
        }

        #endregion

        #region QualifiedTableName

        private readonly String m_QualifiedTableName;

        /// <summary>Gets the qualified name used in SQL statement.</summary>
        public String QualifiedTableName {
            get { return m_QualifiedTableName; }
        } 

        #endregion

        #region Entity

        #region EntityNamespace

        private readonly String m_EntityNamespace;

        /// <summary>Gets the namespace of the entity class.</summary>
        public String EntityNamespace { get { return m_EntityNamespace; } }

        #endregion

        #region EntityName

        private readonly String m_EntityName;

        /// <summary>Gets the class name of the generated entity.</summary>
        public string EntityName { get { return m_EntityName; } }

        #endregion

        #region EntityFullName

        private readonly String m_EntityFullName;

        /// <summary>Gets the <see cref="Type.FullName"/> qualified class name.</summary>
        public String EntityFullName { get { return m_EntityFullName; } }

        #endregion

        #region EntityQualifiedName

        private readonly String m_EntityQualifiedName;

        /// <summary>Gets the name of the <see cref="Type.AssemblyQualifiedName"/> of the generated data class.</summary>
        public String EntityQualifiedName { get { return m_EntityQualifiedName; } }

        #endregion

        #region EntityPartialDecl

        private readonly String m_EntityPartialDecl;

        public String EntityPartialDecl {
            get { return m_EntityPartialDecl; }
        }

        #endregion

        #region EntityBaseDecl

        private readonly String m_EntityBaseDecl;

        /// <summary>Gets the declaraction string of the data class.</summary>
        /// <remarks>Something like "public partial MyClass : MyBaseClass".</remarks>
        public String EntityBaseDecl { get { return m_EntityBaseDecl; } }

        #endregion

        #endregion

        #region DaoIntf

        #region DaoIntfNamespace

        private readonly String m_DaoIntfNamespace;

        /// <summary>Gets the namespace of the DAO interface.</summary>
        public String DaoIntfNamespace { get { return m_DaoIntfNamespace; } }

        #endregion

        #region DaoIntfName

        private readonly String m_DaoIntfName;

        /// <summary>Gets the name of the DAO interface.</summary>
        public String DaoIntfName { get { return m_DaoIntfName; } }

        #endregion

        #region DaoIntfFullName

        private readonly String m_DaoIntfFullName;

        /// <summary>Gets the <see cref="Type.FullName"/> of the DAO interface.</summary>
        public String DaoIntfFullName { get { return m_DaoIntfFullName; } }

        #endregion

        #region DaoIntfQualifiedName

        private readonly String m_DaoIntfQualifiedName;

        /// <summary>Gets the <see cref="Type.AssemblyQualifiedName"/> of the DAO interface.</summary>
        public String DaoIntfQualifiedName { get { return m_DaoIntfQualifiedName; } }

        #endregion

        #region DaoIntfPartialDecl

        private readonly String m_DaoIntfPartialDecl;

        public String DaoIntfPartialDecl {
            get { return m_DaoIntfPartialDecl; }
        }

        #endregion

        #region DaoIntfBaseDecl

        private readonly String m_DaoIntfBaseDecl;

        /// <summary>Gets the base types declaraction.</summary>
        public String DaoIntfBaseDecl {
            get { return m_DaoIntfBaseDecl; }
        } 

        #endregion

        #endregion

        #region DaoImpl

        #region DaoImplNamespace

        private readonly String m_DaoImplNamespace;

        /// <summary>Gets the namespace of the DAO class.</summary>
        public String DaoImplNamespace { get { return m_DaoImplNamespace; } }

        #endregion

        #region DaoImplName

        private readonly String m_DaoImplName;

        /// <summary>Gets the name of the DAO class.</summary>
        public String DaoImplName { get { return m_DaoImplName; } }

        #endregion

        #region DaoImplFullName

        private readonly String m_DaoImplFullName;

        /// <summary>Gets the <see cref="Type.FullName"/> of the DAO class.</summary>
        public String DaoImplFullName { get { return m_DaoImplFullName; } }

        #endregion

        #region DaoImplQualifiedName

        private readonly String m_DaoImplQualifiedName;

        /// <summary>Gets the <see cref="Type.AssemblyQualifiedName"/> of the DAO class.</summary>
        public String DaoImplQualifiedName { get { return m_DaoImplQualifiedName; } }

        #endregion

        #region DaoImplPartialDecl

        private readonly String m_DaoImplPartialDecl;

        public String DaoImplPartialDecl {
            get { return m_DaoImplPartialDecl; }
        }

        #endregion

        #region DaoImplBaseDecl

        private readonly String m_DaoImplBaseDecl;

        /// <summary>Gets the base types declaraction.</summary>
        public String DaoImplBaseDecl {
            get { return m_DaoImplBaseDecl; }
        }

        #endregion

        #region SqlMapper

        private readonly String m_SqlMapper;

        /// <summary>Gets the expression of SqlMapper.</summary>
        public String SqlMapper {
            get { return m_SqlMapper; }
        }

        #endregion

        #endregion

        #region Column collections

        #region Columns

        private readonly IList<ColumnInfo> m_Columns;

        /// <summary>Gets a collection of all <see cref="ColumnSchema"/>s.</summary>
        public IList<ColumnInfo> Columns { get { return m_Columns; } }

        #endregion

        #region PkColumns

        /// <summary>A list of all <see cref="ColumnInfo"/> for primary-key <see cref="ColumnSchema"/>s.</summary>
        private readonly IList<ColumnInfo> m_PkColumns;

        /// <summary>Gets a list of all <see cref="ColumnInfo"/> for primary-key <see cref="ColumnSchema"/>s.</summary>
        public IList<ColumnInfo> PkColumns { get { return m_PkColumns; } }

        #endregion

        #region NonPkColumns

        /// <summary>A list of all <see cref="ColumnInfo"/> for non-primary-key <see cref="ColumnSchema"/>s.</summary>
        private readonly IList<ColumnInfo> m_NonPkColumns;

        /// <summary>Gets a collection of all <see cref="ColumnSchema"/>s except primary-key columns.</summary>
        public IList<ColumnInfo> NonPkColumns { get { return m_NonPkColumns; } }

        #endregion

        #region FinderColumns

        /// <summary>A list of all <see cref="ColumnInfo"/> for generating additional "FindByXXX" methods.</summary>
        private readonly IList<ColumnInfo> m_FinderColumns;

        /// <summary>Gets a collection of <see cref="ColumnInfo"/>s used in FindBy methods.</summary>
        /// <remarks>The result should not include: Primary-key columns, Lob columns, array (such as Byte[]) fields.</remarks>
        public IList<ColumnInfo> FinderColumns { get { return m_FinderColumns; } }

        #endregion

        #region LobColumns

        /// <summary>A list of all <see cref="ColumnInfo"/> for LOB <see cref="ColumnSchema"/>s.</summary>
        private readonly IList<ColumnInfo> m_LobColumns;

        /// <summary>Gets a collection of Lob <see cref="ColumnSchema"/>.</summary>
        public IList<ColumnInfo> LobColumns { get { return m_LobColumns; } }

        #endregion

        #region NonLobColumns

        /// <summary>A list of all <see cref="ColumnInfo"/> for Non-LOB <see cref="ColumnSchema"/>s.</summary>
        private readonly IList<ColumnInfo> m_NonLobColumns;

        /// <summary>Gets a collection of non-Lob <see cref="ColumnSchema"/>.</summary>
        public IList<ColumnInfo> NonLobColumns { get { return m_NonLobColumns; } }

        #endregion

        public String GetClrParamsDecl(IList<ColumnInfo> columns) {
            if (columns == null) throw new ArgumentNullException("columns");
            String result = String.Empty;
            for (int i = 0; i < columns.Count; i++) {
                if (i > 0)
                    result = result + ", ";
                result = result + columns[i].ClrParamDecl;
            }
            return result;
        }

        public String GetClrParamNames(IList<ColumnInfo> columns) {
            if (columns == null) throw new ArgumentNullException("columns");
            String result = String.Empty;
            for (int i = 0; i < columns.Count; i++) {
                if (i > 0)
                    result = result + ", ";
                result = result + columns[i].ClrParamName;
            }
            return result;
        }

        #endregion

        #region BatisNamespace

        /// <summary>Gets the value of the namespace attribute for the "sqlMap" element in Batis map file.</summary>
        public String BatisNamespace {
            get { return this.EntityName; }
        }

        #endregion

        #region Dao method names

        [Category(Categories.DebugInfo)]
        public string GetCountMethod {
            get { return String.Format(DaoMethodSetOption.GetCountOption.IdFmt, String.Empty); }
        }

        [Category(Categories.DebugInfo)]
        public string FindMethod {
            get { return String.Format(DaoMethodSetOption.FindOption.IdFmt, String.Empty); }
        }

        [Category(Categories.DebugInfo)]
        public string FindNonLobMethod {
            get { return String.Format(DaoMethodSetOption.FindNonLobOption.IdFmt, String.Empty); }
        }

        [Category(Categories.DebugInfo)]
        public string FindAllMethod {
            get { return String.Format(DaoMethodSetOption.FindAllOption.IdFmt, String.Empty); }
        }

        [Category(Categories.DebugInfo)]
        public string FindNonLobAllMethod {
            get { return String.Format(DaoMethodSetOption.FindNonLobAllOption.IdFmt, String.Empty); }
        }

        public string FindByMethod(ColumnInfo column) {
            String name = column.PropName;
            return String.Format(DaoMethodSetOption.FindByOption.IdFmt, String.Empty, name);
        }

        public string FindNonLobByMethod(ColumnInfo column) {
            String name = column.PropName;
            return String.Format(DaoMethodSetOption.FindNonLobByOption.IdFmt, String.Empty, name);
        }

        [Category(Categories.DebugInfo)]
        public string InsertMethod {
            get { return String.Format(DaoMethodSetOption.InsertOption.IdFmt, String.Empty); }
        }

        [Category(Categories.DebugInfo)]
        public string UpdateMethod {
            get { return String.Format(DaoMethodSetOption.UpdateOption.IdFmt, String.Empty); }
        }

        [Category(Categories.DebugInfo)]
        public string DeleteMethod {
            get { return String.Format(DaoMethodSetOption.DeleteOption.IdFmt, String.Empty); }
        }

        public string DeleteByMethod(ColumnInfo column) {
            String name = column.PropName;
            return String.Format(DaoMethodSetOption.DeleteByOption.IdFmt, String.Empty, name);
        }

        [Category(Categories.DebugInfo)]
        public string ReloadMethod {
            get { return String.Format(DaoMethodSetOption.ReloadOption.IdFmt, String.Empty); }
        }

        #endregion

        #region Statement ids

        #region IdBase

        private readonly String m_IdBase;

        /// <summary>Gets the base part (i.e. prefix part) of a statement id.</summary>
        protected string IdBase { get { return m_IdBase; } }

        #endregion

        [Category(Categories.DebugInfo)]
        public string GetCountStmtId {
            get { return String.Format(DaoMethodSetOption.GetCountOption.IdFmt, IdBase); }
        }

        [Category(Categories.DebugInfo)]
        public string FindStmtId {
            get { return String.Format(DaoMethodSetOption.FindOption.IdFmt, IdBase); }
        }

        [Category(Categories.DebugInfo)]
        public string FindNonLobStmtId {
            get { return String.Format(DaoMethodSetOption.FindNonLobOption.IdFmt, IdBase); }
        }

        [Category(Categories.DebugInfo)]
        public string FindAllStmtId {
            get { return String.Format(DaoMethodSetOption.FindAllOption.IdFmt, IdBase); }
        }

        [Category(Categories.DebugInfo)]
        public string FindNonLobAllStmtId {
            get { return String.Format(DaoMethodSetOption.FindNonLobAllOption.IdFmt, IdBase); }
        }

        public string FindByStmtId(ColumnInfo column) {
            String name = column.PropName;
            return String.Format(DaoMethodSetOption.FindByOption.IdFmt, IdBase, name);
        }

        public string FindNonLobByStmtId(ColumnInfo column) {
            String name = column.PropName;
            return String.Format(DaoMethodSetOption.FindNonLobByOption.IdFmt, IdBase, name);
        }

        [Category(Categories.DebugInfo)]
        public string InsertStmtId {
            get { return String.Format(DaoMethodSetOption.InsertOption.IdFmt, IdBase); }
        }

        [Category(Categories.DebugInfo)]
        public string UpdateStmtId {
            get { return String.Format(DaoMethodSetOption.UpdateOption.IdFmt, IdBase); }
        }

        [Category(Categories.DebugInfo)]
        public string DeleteStmtId {
            get { return String.Format(DaoMethodSetOption.DeleteOption.IdFmt, IdBase); }
        }

        public string DeleteByStmtId(ColumnInfo column) {
            String name = column.PropName;
            return String.Format(DaoMethodSetOption.DeleteByOption.IdFmt, IdBase, name);
        }

        [Category(Categories.DebugInfo)]
        public string ReloadStmtId {
            get { return String.Format(DaoMethodSetOption.ReloadOption.IdFmt, IdBase); }
        }

        [Category(Categories.DebugInfo)]
        public string FullResultMapId {
            get { return String.Format(SqlOption.FullResultMapIdFmt, IdBase); }
        }

        [Category(Categories.DebugInfo)]
        public string NonLobResultMapId {
            get { return String.Format(SqlOption.NonLobResultMapIdFmt, IdBase); }
        }

        #endregion

        #region HasLob

        private readonly bool m_HasLob;

        /// <summary>Indicates if contains Lob columns.</summary>
        public bool HasLob { get { return m_HasLob; } }

        #endregion

        #region Static utility methods

        private static String GetEntityName(String tableName, EntityOption option) {
            String s = tableName;

            if (CSHelper.IsNotBlank(option.PrefixToTrim)) {
                s = CSHelper.StripPrefix(s, option.PrefixToTrim);
            }

            if (CSHelper.IsNotBlank(option.SuffixToTrim)) {
                s = CSHelper.StripSuffix(s, option.SuffixToTrim);
            }

            if (option.InPascalCase) {
                s = CSHelper.GetPascalCase(s);
            }

            if (option.InSingular && s.EndsWith("s")) {
                s = s.TrimEnd('s');
            }

            s = CSHelper.GetCSharpIdentifier(s);

            s = String.Format(option.NameFmt, s);

            return s;
        }

        private static String GetEntityDecl(String entityName, EntityOption option) {
            String s = String.Format("{0}class {1}",
                option.IsPartial ? "partial " : "", entityName);
            if (CSHelper.IsNotBlank(option.BaseType))
                s = s + " : " + option.BaseType;
            return s;
        }

        private static String GetIdBase(String entityName, String ns, SqlOption option) {
            switch (option.IdPattern) {
                case StmtIdPattern.Local:
                    return String.Empty;
                case StmtIdPattern.Classified:
                    return entityName + option.IdDelimiter;
                case StmtIdPattern.NamespaceClassified:
                    //Something like "Hello-World-MyClass-"
                    return ns.Replace(".", option.IdDelimiter)
                        + option.IdDelimiter
                        + entityName
                        + option.IdDelimiter;
                default:
                    throw new NotImplementedException(String.Format("Unknown IdPattern [{0}]", option.IdPattern.ToString()));
            }
        }

        #endregion

    }

    #endregion

    #region BatisBaseTemplate

    /// <summary><c>BatisBaseTemplate</c> is the CodeSmith template for accepting table generation options.</summary>
    public abstract class BatisBaseTemplate : CodeTemplate, ITableBuilderOption {

        #region IBatisOptions Properties for design-time editing

        #region EntityOption

        private EntityOption m_EntityOption = new EntityOption();

        [Category(Categories.Options)]
        [Description("Data class generation options")]
        public EntityOption EntityOption {
            get { return m_EntityOption; }
            set { m_EntityOption = value; }
        }

        #endregion

        #region MemberOption

        private MemberOption m_MemberOption = new MemberOption();

        [Category(Categories.Options)]
        [Description("Member property/variable options")]
        public MemberOption MemberOption {
            get { return m_MemberOption; }
            set { m_MemberOption = value; }
        }

        #endregion

        #region SqlOption

        private SqlOption m_SqlOption = new SqlOption();

        [Category(Categories.Options)]
        [Description("SQL statement generation options")]
        public SqlOption SqlOption {
            get { return m_SqlOption; }
            set { m_SqlOption = value; }
        }

        #endregion

        #region DaoIntfOption

        private DaoIntfOption m_DaoIntfOption = new DaoIntfOption();

        [Category(Categories.Options)]
        [Description("DAO interface generation options")]
        public DaoIntfOption DaoIntfOption {
            get { return m_DaoIntfOption; }
            set { m_DaoIntfOption = value; }
        }

        #endregion

        #region DaoImplOption

        private DaoImplOption m_DaoImplOption = new DaoImplOption();

        [Category(Categories.Options)]
        [Description("DAO implementation generation options")]
        public DaoImplOption DaoImplOption {
            get { return m_DaoImplOption; }
            set { m_DaoImplOption = value; }
        }

        #endregion

        #region DaoMethodSetOption

        private DaoMethodSetOption m_DaoMethodSetOption = new DaoMethodSetOption();

        [Category(Categories.Options)]
        [Description("DAO data access methods generation options")]
        public DaoMethodSetOption DaoMethodSetOption {
            get { return m_DaoMethodSetOption; }
            set { m_DaoMethodSetOption = value; }
        }

        #endregion

        #endregion

        #region FileOption

        private FileOption m_FileOption;

        /// <summary>This property is defined virtual for enabling sub classes add Browseable(false) attribute.</summary>
        [Category(Categories.Output)]
        [Description("File generation option")]
        public virtual FileOption FileOption {
            get {
                if (m_FileOption == null) {
                    m_FileOption = GetDefaultFileOption();
                }
                return m_FileOption;
            }
            set { m_FileOption = value; }
        }

        protected abstract FileOption GetDefaultFileOption();

        #endregion

        #region OutputDir

        [Category(Categories.Output)]
        protected virtual String OutputDir {
            get {
                String s = FileOption.Dir;

                if (String.IsNullOrEmpty(s))
                    return CodeTemplateInfo.DirectoryName;
                
                if (Path.IsPathRooted(s))
                    return s;

                return Path.Combine(CodeTemplateInfo.DirectoryName, s);
            }
        }

        #endregion

        #region OutputFile

        [Category(Categories.Output)]
        protected virtual String OutputFile {
            get {
                String s = Path.Combine(OutputDir, FileName);
                s = Path.GetFullPath(s);
                return s;
            }
        }

        #endregion

        #region FileName

        [Category(Categories.Output)]
        public String FileName {
            get { return String.Format(FileOption.FileNameFmt, GetFileNameFormatArg()); }
        }

        protected abstract String GetFileNameFormatArg();

        #endregion

        #region OnPostRender

        /// <summary>Write the <paramref name="result"/> to the file specified by <see cref="FileOption"/>.</summary>
        protected override void OnPostRender(string result) {
            //Debugger.Break();
            if (FileOption.Save) {
                if ((OutputFile != null) && (OutputFile.Length > 0)) {
                    String path = Path.GetDirectoryName(OutputFile);
                    
                    if (!Directory.Exists(path)) {
                        Console.WriteLine("Creating directory : " + path);
                        Directory.CreateDirectory(path);
                    }

                    FileStream stm = new FileStream(this.OutputFile, FileMode.Create, FileAccess.Write); ;
                    try {
                        byte[] buf = Encoding.UTF8.GetBytes(result);
                        stm.Write(buf, 0, buf.Length);
                        Debug.WriteLine(String.Format("File generated: {0}", this.OutputFile));
                    } finally {
                        if (stm != null) {
                            stm.Close();
                        }
                    }
                }
            }
            base.OnPostRender(result);
        }

        #endregion

    }

    #endregion

    #region TableTemplate

    /// <summary><c>TableTemplate</c> enables generations in per-table-manner.</summary>
    public abstract class TableTemplate : BatisBaseTemplate {

        #region SourceTable

        private TableSchema m_SourceTable;

        [Category(Categories.Data)]
        [Description("(Required) Select the table to generate.")]
        public TableSchema SourceTable {
            get {
                if (m_Builder != null) return m_Builder.SourceTable;
                return m_SourceTable; }
            set { m_SourceTable = value; }
        }

        #endregion

        #region Builder

        private TableBuilder m_Builder;

        /// <summary>Gets or sets the <see cref="TableBuilder"/>.</summary>
        /// <remarks>A instance of <c>TableBuilder</c> will be automatically created if not assigned before renderring
        /// in <see cref="OnPreRender"/>.</remarks>
        [Browsable(false)]
        public TableBuilder Builder {
            get { return m_Builder; }
            set { m_Builder = value; }
        }

        #endregion

        protected override void OnPreRender() {
            if (Builder == null) {
                Builder = new TableBuilder(SourceTable, this);
                Debug.WriteLine("Builder created in OnPreRender");
            }
            base.OnPreRender();
        }

        protected override String GetFileNameFormatArg() {
            return Builder.EntityName;
        }

    }

    #endregion

    #region TableCompositeTemplate

    /// <summary><c>TableCompositeTemplate</c> generates multiple files for a <see cref="TableSchema"/>.</summary>
    public class TableCompositeTemplate : TableTemplate {

        #region FileSetOption

        private FileSetOption m_FileSetOption = new FileSetOption();

        [Category(Categories.Output)]
        [Description("File option for each generated file type.")]
        public FileSetOption FileSetOption {
            get { return m_FileSetOption; }
            set { m_FileSetOption = value; }
        }

        #endregion

        /// <summary>In default, not save to a single file.</summary>
        protected override FileOption GetDefaultFileOption() {
            return new FileOption(false, @"Output\TableComposite", "{0}.TableComposite.txt");
        }

    }

    #endregion

    #region DatabaseCompositeTemplate

    public class DatabaseCompositeTemplate : BatisBaseTemplate {

        #region FileSetOption

        private FileSetOption m_FileSetOption = new FileSetOption();

        [Category(Categories.Output)]
        [Description("File option for each generated file type.")]
        public FileSetOption FileSetOption {
            get { return m_FileSetOption; }
            set { m_FileSetOption = value; }
        }

        #endregion

        protected override String GetFileNameFormatArg() {
            if (SourceTables != null && SourceTables.Count > 0)
                return SourceTables[0].Database.Name;
            return String.Empty;
        }

        /// <summary>In default, not save to a single file.</summary>
        protected override FileOption GetDefaultFileOption() {
            return new FileOption(false, "Output", "{0}.Database.txt");
        }

        #region SourceTables

        private TableSchemaCollection m_SourceTables = new TableSchemaCollection();

        /// <summary>Gets or sets a list of <see cref="TableSchema"/>s for generating.</summary>
        [Category(Categories.Data)]
        [Description("Gets or sets a list of tables for generating.")]
        public TableSchemaCollection SourceTables {
            get { return m_SourceTables; }
            set { m_SourceTables = value; }
        }

        #endregion

    }

    #endregion

    #region Utilities

    internal static class CSHelper {

        #region GetPascalCase/GetCamelCase

        public static string GetPascalCase(string s) {
            char[] delimiters = { '_', ' ' };
            string[] parts = s.Split(delimiters);
            string result = "";
            foreach (string part in parts) {
                if (part.Length > 0)
                    result += part.Substring(0, 1).ToUpper() + part.Substring(1);
            }
            return result;
        }

        public static string GetCamelCase(string s) {
            String pascalName = GetPascalCase(s);
            return pascalName.Substring(0, 1).ToLower() + pascalName.Substring(1);
        }

        #endregion

        #region StripPrefix/StripSuffix

        public static string StripPrefix(String s, String prefix) {
            if ((s != null) && (s.Length > 0) && (prefix != null) && (prefix.Length > 0)) {
                if (s.StartsWith(prefix))
                    return s.Substring(prefix.Length);
            }
            return s;
        }

        public static string StripSuffix(String s, String suffix) {
            if ((s != null) && (s.Length > 0) && (suffix != null) && (suffix.Length > 0)) {
                if (s.EndsWith(suffix))
                    return s.Substring(0, s.Length - suffix.Length);
            }
            return s;
        }

        #endregion

        #region IsNotBlank/Trim

        public static bool IsNotBlank(String s) {
            return (s != null) && (s.Trim().Length > 0);
        }

        /// <summary>Gets a trimmed string.</summary>
        public static String Trim(String s, bool checkNonBlank) {
            String result = (s != null) ? s.Trim() : s;
            if ((String.IsNullOrEmpty(s)) && (checkNonBlank))
                throw new ArgumentException("The input string is null, or contains whitespace only!");
            return result;
        }

        #endregion

        #region ToStringArray

        public delegate String StringConverterDelegate(Object o);

        /// <summary>Converts a list of object to an <see cref="String"/> array.</summary>
        public static String[] ToArray(IList list, StringConverterDelegate converter) {
            if (list == null) throw new ArgumentNullException("list");
            if (converter == null) throw new ArgumentNullException("converter");
            String[] result = new String[list.Count];
            for (int i = 0; i < list.Count; i++) {
                result[i] = converter(list[i]);
            }
            return result;
        }

        #endregion

        public static String GetMemberAccessName(MemberAccess access) {
            return access.ToString().ToLower();
        }

        #region GetQualifiedName, GetAssemblyQualifiedName

        public static String GetQualifiedName(String className, String ns) {
            ns = Trim(ns, false);
            if (!String.IsNullOrEmpty(ns))
                ns = ns + ".";

            return ns + Trim(className, true);
        }

        public static String GetAssemblyQualifiedName(String qualifiedName, String assemblyName) {
            String temp = Trim(assemblyName, false);
            if (!String.IsNullOrEmpty(temp))
                return qualifiedName + ", " + temp;
            return qualifiedName;
        }

        #endregion

        #region GetCSharpIdentifier

        public static string GetCSharpIdentifier(String s) {
            return s = Trim(s, true).Replace(" ", "");
        }

        #endregion

    }

    #endregion

}
