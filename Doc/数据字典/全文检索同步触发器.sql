Create Trigger HBTrigger_DocInfo_Update
    On D_DocInfo                        
    for Update                         

    As                                       
    DECLARE @updateFields nvarchar(4000)
    set @updateFields = ''

    if Update(UserId)          
    begin
    set @updateFields = @updateFields + 'UserId,'
    end       

    if Update(Title)          
    begin
    set @updateFields = @updateFields + 'Title,'
    end       

    if Update(CateId)          
    begin
    set @updateFields = @updateFields + 'CateId,'
    end       

    if Update(Description)          
    begin
    set @updateFields = @updateFields + 'Description,'
    end       

    if Update(Tags)          
    begin
    set @updateFields = @updateFields + 'Tags,'
    end   
    
    if Update(Price)          
    begin
    set @updateFields = @updateFields + 'Price,'
    end 
    
    if Update(ViewCount)          
    begin
    set @updateFields = @updateFields + 'ViewCount,'
    end 
    
    if Update(FavCount)          
    begin
    set @updateFields = @updateFields + 'FavCount,'
    end 
    
    if Update(UpCount)          
    begin
    set @updateFields = @updateFields + 'UpCount,'
    end 
    
    if Update(CommentCount)          
    begin
    set @updateFields = @updateFields + 'CommentCount,'
    end 
    
    if Update(IsAudit)          
    begin
    set @updateFields = @updateFields + 'IsAudit,'
    END
    
    if Update(ThumbnailUrl)          
    begin
    set @updateFields = @updateFields + 'ThumbnailUrl,'
    end

    if @updateFields <> ''
    begin
    insert into HBTrigger_EnglishNews select DocId, 'Update',  @updateFields from Inserted
    end
