/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2005                    */
/* Created on:     2011-1-2 22:37:16                            */
/*==============================================================*/


if exists (select 1
            from  sysobjects
           where  id = object_id('D_Comment')
            and   type = 'U')
   drop table D_Comment
go

if exists (select 1
            from  sysobjects
           where  id = object_id('D_DocInfo')
            and   type = 'U')
   drop table D_DocInfo
go

if exists (select 1
            from  sysobjects
           where  id = object_id('D_FileInfo')
            and   type = 'U')
   drop table D_FileInfo
go

if exists (select 1
            from  sysobjects
           where  id = object_id('D_Rel_DocTag')
            and   type = 'U')
   drop table D_Rel_DocTag
go

if exists (select 1
            from  sysobjects
           where  id = object_id('D_Tag')
            and   type = 'U')
   drop table D_Tag
go

if exists (select 1
            from  sysobjects
           where  id = object_id('M_Bill')
            and   type = 'U')
   drop table M_Bill
go

if exists (select 1
            from  sysobjects
           where  id = object_id('M_Catalog')
            and   type = 'U')
   drop table M_Catalog
go

if exists (select 1
            from  sysobjects
           where  id = object_id('M_Favorite')
            and   type = 'U')
   drop table M_Favorite
go

if exists (select 1
            from  sysobjects
           where  id = object_id('M_Message')
            and   type = 'U')
   drop table M_Message
go

if exists (select 1
            from  sysobjects
           where  id = object_id('M_Purchase')
            and   type = 'U')
   drop table M_Purchase
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ManageUser')
            and   type = 'U')
   drop table ManageUser
go

if exists (select 1
            from  sysobjects
           where  id = object_id('N_News')
            and   type = 'U')
   drop table N_News
go

if exists (select 1
            from  sysobjects
           where  id = object_id('S_Catalog')
            and   type = 'U')
   drop table S_Catalog
go

if exists (select 1
            from  sysobjects
           where  id = object_id('S_Config')
            and   type = 'U')
   drop table S_Config
go

if exists (select 1
            from  sysobjects
           where  id = object_id('S_FriendLink')
            and   type = 'U')
   drop table S_FriendLink
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_JoinDoc')
            and   type = 'U')
   drop table T_JoinDoc
go

if exists (select 1
            from  sysobjects
           where  id = object_id('T_ReqDoc')
            and   type = 'U')
   drop table T_ReqDoc
go

if exists (select 1
            from  sysobjects
           where  id = object_id('U_UserInfo')
            and   type = 'U')
   drop table U_UserInfo
go

/*==============================================================*/
/* Table: D_Comment                                             */
/*==============================================================*/
create table D_Comment (
   CommentId            int                  identity,
   DocId                int                  not null,
   Content              varchar(1000)        not null,
   CreateTime           datetime             not null,
   UserId               int                  not null,
   constraint PK_D_COMMENT primary key (CommentId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '评论ID',
   'user', @CurrentUser, 'table', 'D_Comment', 'column', 'CommentId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档ID',
   'user', @CurrentUser, 'table', 'D_Comment', 'column', 'DocId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '内容',
   'user', @CurrentUser, 'table', 'D_Comment', 'column', 'Content'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'D_Comment', 'column', 'CreateTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '评论人',
   'user', @CurrentUser, 'table', 'D_Comment', 'column', 'UserId'
go

/*==============================================================*/
/* Table: D_DocInfo                                             */
/*==============================================================*/
create table D_DocInfo (
   DocId                int                  identity,
   Title                varchar(100)         not null,
   DocType              int                  not null,
   UserId               int                  not null,
   CateId               int                  null,
   UserCateId           int                  null,
   Description          varchar(2000)        null,
   Tags                 varchar(500)         null,
   Price                decimal              null,
   Length               int                  null,
   ViewCount            int                  null default 0,
   FavCount             int                  null default 0,
   UpCount              int                  null default 0,
   CreateTime           datetime             null default getdate(),
   IsAudit              bit                  null default 0,
   IsRecommend          bit                  null default 0,
   IsTaskDoc            bit                  null default 0,
   constraint PK_D_DOCINFO primary key (DocId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档表',
   'user', @CurrentUser, 'table', 'D_DocInfo'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档ID',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'DocId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档标题',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'Title'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档类型doc/pdf..',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'DocType'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '所有者',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '系统分类编号',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'CateId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '所属文件夹编号',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'UserCateId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档简介',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'Description'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '标签',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'Tags'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '价格',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'Price'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档大小',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'Length'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '浏览数',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'ViewCount'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '收藏数',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'FavCount'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '顶数',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'UpCount'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '上传时间',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'CreateTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '是否通过审核',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'IsAudit'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '是否精选文档',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'IsRecommend'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '是否悬赏文档',
   'user', @CurrentUser, 'table', 'D_DocInfo', 'column', 'IsTaskDoc'
go

/*==============================================================*/
/* Table: D_FileInfo                                            */
/*==============================================================*/
create table D_FileInfo (
   Fid                  uniqueidentifier     not null,
   FileType             int                  null,
   FileName             varchar(200)         null,
   OwnerId              int                  not null,
   FilePath             varchar(200)         not null,
   CreateTime           datetime             not null default getdate(),
   constraint PK_D_FILEINFO primary key (Fid)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件信息表',
   'user', @CurrentUser, 'table', 'D_FileInfo'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件ID',
   'user', @CurrentUser, 'table', 'D_FileInfo', 'column', 'Fid'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件类型',
   'user', @CurrentUser, 'table', 'D_FileInfo', 'column', 'FileType'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件原始名称',
   'user', @CurrentUser, 'table', 'D_FileInfo', 'column', 'FileName'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '所有者ID',
   'user', @CurrentUser, 'table', 'D_FileInfo', 'column', 'OwnerId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文件路径',
   'user', @CurrentUser, 'table', 'D_FileInfo', 'column', 'FilePath'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'D_FileInfo', 'column', 'CreateTime'
go

/*==============================================================*/
/* Table: D_Rel_DocTag                                          */
/*==============================================================*/
create table D_Rel_DocTag (
   Id                   int                  identity,
   DocId                int                  not null,
   TagId                int                  not null,
   constraint PK_D_REL_DOCTAG primary key (Id)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '自增ID',
   'user', @CurrentUser, 'table', 'D_Rel_DocTag', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档ID',
   'user', @CurrentUser, 'table', 'D_Rel_DocTag', 'column', 'DocId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '标签ID',
   'user', @CurrentUser, 'table', 'D_Rel_DocTag', 'column', 'TagId'
go

/*==============================================================*/
/* Table: D_Tag                                                 */
/*==============================================================*/
create table D_Tag (
   TagId                int                  identity,
   Tag                  varchar(20)          not null,
   UseCount             int                  not null default 0,
   constraint PK_D_TAG primary key (TagId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '标签ID',
   'user', @CurrentUser, 'table', 'D_Tag', 'column', 'TagId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '标签内容',
   'user', @CurrentUser, 'table', 'D_Tag', 'column', 'Tag'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '使用次数',
   'user', @CurrentUser, 'table', 'D_Tag', 'column', 'UseCount'
go

/*==============================================================*/
/* Table: M_Bill                                                */
/*==============================================================*/
create table M_Bill (
   Bid                  int                  identity,
   UserId               int                  not null,
   CreateTime           datetime             not null default getdate(),
   Direct               int                  not null,
   Remark               varchar(500)         not null,
   Status               int                  not null,
   Price                decimal              not null,
   constraint PK_M_BILL primary key (Bid)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '账单',
   'user', @CurrentUser, 'table', 'M_Bill'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '账单ID',
   'user', @CurrentUser, 'table', 'M_Bill', 'column', 'Bid'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户ID',
   'user', @CurrentUser, 'table', 'M_Bill', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'M_Bill', 'column', 'CreateTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '账单方向',
   'user', @CurrentUser, 'table', 'M_Bill', 'column', 'Direct'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '账单备注',
   'user', @CurrentUser, 'table', 'M_Bill', 'column', 'Remark'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '状态',
   'user', @CurrentUser, 'table', 'M_Bill', 'column', 'Status'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '发生金额',
   'user', @CurrentUser, 'table', 'M_Bill', 'column', 'Price'
go

/*==============================================================*/
/* Table: M_Catalog                                             */
/*==============================================================*/
create table M_Catalog (
   CateId               int                  identity,
   UserId               int                  not null,
   CateText             varchar(50)          not null,
   CreateTime           datetime             not null default getdate(),
   CatalogType          int                  not null,
   constraint PK_M_CATALOG primary key (CateId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '我的文档目录',
   'user', @CurrentUser, 'table', 'M_Catalog'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '分类ID',
   'user', @CurrentUser, 'table', 'M_Catalog', 'column', 'CateId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户ID',
   'user', @CurrentUser, 'table', 'M_Catalog', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '分类名称',
   'user', @CurrentUser, 'table', 'M_Catalog', 'column', 'CateText'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'M_Catalog', 'column', 'CreateTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '上传分类或收藏分类',
   'user', @CurrentUser, 'table', 'M_Catalog', 'column', 'CatalogType'
go

/*==============================================================*/
/* Table: M_Favorite                                            */
/*==============================================================*/
create table M_Favorite (
   FavId                int                  identity,
   UserId               int                  not null,
   DocId                int                  not null,
   CreateTime           datetime             not null default getdate(),
   FavCateId            int                  null,
   FavCatalog           varchar(50)          null,
   constraint PK_M_FAVORITE primary key (FavId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '我的收藏',
   'user', @CurrentUser, 'table', 'M_Favorite'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '收藏ID',
   'user', @CurrentUser, 'table', 'M_Favorite', 'column', 'FavId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户ID',
   'user', @CurrentUser, 'table', 'M_Favorite', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档ID',
   'user', @CurrentUser, 'table', 'M_Favorite', 'column', 'DocId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '收藏时间',
   'user', @CurrentUser, 'table', 'M_Favorite', 'column', 'CreateTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '收藏分类ID',
   'user', @CurrentUser, 'table', 'M_Favorite', 'column', 'FavCateId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '收藏分类名称',
   'user', @CurrentUser, 'table', 'M_Favorite', 'column', 'FavCatalog'
go

/*==============================================================*/
/* Table: M_Message                                             */
/*==============================================================*/
create table M_Message (
   Mid                  bigint               identity,
   SenderId             int                  null,
   RecieverId           int                  null,
   Title                varchar(200)         null,
   Content              varchar(1000)        null,
   CreateTime           datetime             null,
   IsRead               bit                  null,
   Mtype                int                  not null,
   RefId                int                  null,
   SendDeleteFlag       bit                  null,
   RecieveDeleteFlag    bit                  null,
   constraint PK_M_MESSAGE primary key (Mid)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '系统消息、通知、站内信',
   'user', @CurrentUser, 'table', 'M_Message'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '消息ID',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'Mid'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '发送方ID',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'SenderId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '接收方ID',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'RecieverId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '标题',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'Title'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '内容',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'Content'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'CreateTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '是否阅读',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'IsRead'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '消息类型',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'Mtype'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '回复ID',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'RefId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '发件人删除标志',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'SendDeleteFlag'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '收件人删除标志',
   'user', @CurrentUser, 'table', 'M_Message', 'column', 'RecieveDeleteFlag'
go

/*==============================================================*/
/* Table: M_Purchase                                            */
/*==============================================================*/
create table M_Purchase (
   Pid                  int                  identity,
   UserId               int                  not null,
   DocId                int                  not null,
   Title                varchar(200)         null,
   PurchaseTime         datetime             null,
   Price                decimal              null,
   Saler                varchar(50)          null,
   constraint PK_M_PURCHASE primary key (Pid)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '我的购买',
   'user', @CurrentUser, 'table', 'M_Purchase'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '购买ID',
   'user', @CurrentUser, 'table', 'M_Purchase', 'column', 'Pid'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户ID',
   'user', @CurrentUser, 'table', 'M_Purchase', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档ID',
   'user', @CurrentUser, 'table', 'M_Purchase', 'column', 'DocId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '文档标题',
   'user', @CurrentUser, 'table', 'M_Purchase', 'column', 'Title'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '购买时间',
   'user', @CurrentUser, 'table', 'M_Purchase', 'column', 'PurchaseTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '消费金额',
   'user', @CurrentUser, 'table', 'M_Purchase', 'column', 'Price'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '上传人',
   'user', @CurrentUser, 'table', 'M_Purchase', 'column', 'Saler'
go

/*==============================================================*/
/* Table: ManageUser                                            */
/*==============================================================*/
create table ManageUser (
   UserName             varchar(20)          not null,
   Password             varchar(100)         not null,
   TrueName             varchar(50)          null,
   Mobile               char(11)             null,
   Level                int                  null,
   CreateTime           datetime             null,
   UpdateTime           datetime             null,
   Remark               varchar(500)         null,
   constraint PK_MANAGEUSER primary key (UserName)
)
go

/*==============================================================*/
/* Table: N_News                                                */
/*==============================================================*/
create table N_News (
   Nid                  int                  identity,
   Title                varchar(200)         null,
   Content              text                 null,
   Catalog              varchar(50)          null,
   constraint PK_N_NEWS primary key (Nid)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '新闻表',
   'user', @CurrentUser, 'table', 'N_News'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '新闻ID',
   'user', @CurrentUser, 'table', 'N_News', 'column', 'Nid'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '标题',
   'user', @CurrentUser, 'table', 'N_News', 'column', 'Title'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '内容',
   'user', @CurrentUser, 'table', 'N_News', 'column', 'Content'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '新闻分类',
   'user', @CurrentUser, 'table', 'N_News', 'column', 'Catalog'
go

/*==============================================================*/
/* Table: S_Catalog                                             */
/*==============================================================*/
create table S_Catalog (
   CatalogId            int                  identity,
   CatalogName          varchar(50)          not null,
   Pid                  int                  null,
   OrderId              int                  not null default 0,
   constraint PK_S_CATALOG primary key (CatalogId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '系统分类',
   'user', @CurrentUser, 'table', 'S_Catalog'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '分类ID',
   'user', @CurrentUser, 'table', 'S_Catalog', 'column', 'CatalogId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '分类名称',
   'user', @CurrentUser, 'table', 'S_Catalog', 'column', 'CatalogName'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '父ID',
   'user', @CurrentUser, 'table', 'S_Catalog', 'column', 'Pid'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '排序号',
   'user', @CurrentUser, 'table', 'S_Catalog', 'column', 'OrderId'
go

/*==============================================================*/
/* Table: S_Config                                              */
/*==============================================================*/
create table S_Config (
   Id                   int                  not null,
   WebName              varchar(100)         null,
   Keywords             varchar(1000)        null,
   Description          varchar(4000)        null,
   constraint PK_S_CONFIG primary key (Id)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '系统全局配置',
   'user', @CurrentUser, 'table', 'S_Config'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'Id',
   'user', @CurrentUser, 'table', 'S_Config', 'column', 'Id'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '网站名称',
   'user', @CurrentUser, 'table', 'S_Config', 'column', 'WebName'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'SEO关键字',
   'user', @CurrentUser, 'table', 'S_Config', 'column', 'Keywords'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'SEO描述',
   'user', @CurrentUser, 'table', 'S_Config', 'column', 'Description'
go

/*==============================================================*/
/* Table: S_FriendLink                                          */
/*==============================================================*/
create table S_FriendLink (
   Fid                  int                  identity,
   Title                varchar(200)         not null,
   LinkUrl              varchar(500)         not null,
   OrderId              int                  not null default 0,
   constraint PK_S_FRIENDLINK primary key (Fid)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '友情链接',
   'user', @CurrentUser, 'table', 'S_FriendLink'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '友情链接ID',
   'user', @CurrentUser, 'table', 'S_FriendLink', 'column', 'Fid'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '显示文本',
   'user', @CurrentUser, 'table', 'S_FriendLink', 'column', 'Title'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '链接地址',
   'user', @CurrentUser, 'table', 'S_FriendLink', 'column', 'LinkUrl'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '排序号',
   'user', @CurrentUser, 'table', 'S_FriendLink', 'column', 'OrderId'
go

/*==============================================================*/
/* Table: T_JoinDoc                                             */
/*==============================================================*/
create table T_JoinDoc (
   JoinId               int                  identity,
   TId                  int                  not null,
   UserId               int                  not null,
   Title                varchar(200)         null,
   DocId                int                  not null,
   constraint PK_T_JOINDOC primary key (JoinId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '悬赏投稿',
   'user', @CurrentUser, 'table', 'T_JoinDoc'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '投稿ID',
   'user', @CurrentUser, 'table', 'T_JoinDoc', 'column', 'JoinId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '悬赏任务ID',
   'user', @CurrentUser, 'table', 'T_JoinDoc', 'column', 'TId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '投稿人',
   'user', @CurrentUser, 'table', 'T_JoinDoc', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '投稿标题',
   'user', @CurrentUser, 'table', 'T_JoinDoc', 'column', 'Title'
go

/*==============================================================*/
/* Table: T_ReqDoc                                              */
/*==============================================================*/
create table T_ReqDoc (
   TId                  int                  identity,
   UserId               int                  not null,
   Title                varchar(200)         not null,
   Description          varchar(1000)        not null,
   Price                decimal              not null,
   CreateTime           datetime             not null default getdate(),
   EndTime              datetime             not null,
   Status               int                  not null,
   constraint PK_T_REQDOC primary key (TId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '悬赏ID',
   'user', @CurrentUser, 'table', 'T_ReqDoc', 'column', 'TId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户ID',
   'user', @CurrentUser, 'table', 'T_ReqDoc', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '悬赏标题',
   'user', @CurrentUser, 'table', 'T_ReqDoc', 'column', 'Title'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '描述',
   'user', @CurrentUser, 'table', 'T_ReqDoc', 'column', 'Description'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '悬赏金额',
   'user', @CurrentUser, 'table', 'T_ReqDoc', 'column', 'Price'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '创建时间',
   'user', @CurrentUser, 'table', 'T_ReqDoc', 'column', 'CreateTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '截止时间',
   'user', @CurrentUser, 'table', 'T_ReqDoc', 'column', 'EndTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '状态',
   'user', @CurrentUser, 'table', 'T_ReqDoc', 'column', 'Status'
go

/*==============================================================*/
/* Table: U_UserInfo                                            */
/*==============================================================*/
create table U_UserInfo (
   UserId               int                  identity,
   NickName             varchar(50)          null,
   Email                varchar(50)          not null,
   Password             varchar(100)         not null,
   TrueName             varchar(20)          null,
   Sex                  bit                  null,
   Birthday             datetime             null,
   JobTitle             varchar(100)         null,
   CompanyType          int                  null,
   Major                int                  null,
   RegTime              datetime             not null,
   RegIp                char(15)             null,
   HeadIcon             varchar(100)         null,
   IsStop               bit                  not null default 0,
   constraint PK_U_USERINFO primary key (UserId)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '用户ID',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'UserId'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '昵称',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'NickName'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '邮箱',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'Email'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '密码',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'Password'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '姓名',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'TrueName'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '性别',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'Sex'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '生日',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'Birthday'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '职业',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'JobTitle'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '单位类别',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'CompanyType'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '专业属性',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'Major'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '注册时间',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'RegTime'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '注册IP',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'RegIp'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '头像',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'HeadIcon'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   '是否停用',
   'user', @CurrentUser, 'table', 'U_UserInfo', 'column', 'IsStop'
go

