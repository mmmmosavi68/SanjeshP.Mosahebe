USE [SanjeshP_Mosahebe]
GO
/****** Object:  StoredProcedure [dbo].[Get_GroupAccess_With_CurrentUserAccess_For_TreeCheckBox]    Script Date: 4/23/2024 2:57:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[Get_GroupAccess_With_CurrentUserAccess_For_TreeCheckBox]
@CurrentUserId uniqueidentifier
,@GroupSelected_GroupId uniqueidentifier
as
Begin
	SELECT distinct(M.Id) AS DISTINCT_MenuID
	, M.Id
	, @CurrentUserId as UserId
	, M.MenuTitle
	, M.ParentId
	, M.PageCode
	, M.ShowMenu
	, M.CssClass
	, M.Icon
	, M.Link
	, M.Area
	, M.ControllerName
	, M.ActionName
	-- لیست منو‌هایی که کاربر جاری به صورت فردی و گروهی به آن دسترسی دارد
	, CONVERT(bit,(SELECT CASE WHEN AM.IsDeleted = 0 THEN 1 WHEN AM.IsDeleted = 1 THEN 0 WHEN AM.IsDeleted IS NULL THEN 0 END)) AS CurentUser_Person_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_User.IsDeleted = 0 THEN 1 WHEN AMG_User.IsDeleted = 1 THEN 0 WHEN AMG_User.IsDeleted IS NULL THEN 0 END)) AS CurentUser_Group_Checkecd
	, ISNULL((SELECT COUNT(Id) 
			FROM dbo.Menus AS MP
			WHERE (ParentId = M.Id)), 0) AS IsParent
	-- لیست منوهایی که گروه انتخاب شده به آن به دسترسی دراد
	, CONVERT(bit,0) as Person_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_Group.IsDeleted = 0 THEN 1 WHEN AMG_Group.IsDeleted = 1 THEN 0 WHEN AMG_Group.IsDeleted IS NULL THEN 0 END)) AS Group_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_Group.IsDeleted = 0 THEN 1 WHEN AMG_Group.IsDeleted = 1 THEN 0 WHEN AMG_Group.IsDeleted IS NULL THEN 0 END)) AS [disabled]
	FROM dbo.Menus AS M 
	LEFT OUTER JOIN dbo.UserAccessMenus AS AM ON AM.MenuId=M.Id and AM.UserId=@CurrentUserId
	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_User ON AMG_User.MenuId=M.Id and AMG_User.GroupId in (select UG.GroupId 
																							from GroupUsers as UG
																							where UG.UserId=@CurrentUserId 
																							and UG.GroupId=@GroupSelected_GroupId
																							and UG.IsActive=1 
																							and UG.IsDeleted=0)
	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_Group ON AMG_Group.MenuId=M.Id and AMG_Group.GroupId=@GroupSelected_GroupId
	WHERE (M.IsDeleted = 0) AND (M.IsActive = 1)
	order by PageCode,ControllerName
--End
End

GO
/****** Object:  StoredProcedure [dbo].[Get_UserCurrentAccess]    Script Date: 4/23/2024 2:57:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--CREATE proc [dbo].[Get_UserCurrentAccess]
--@CurrentUserId uniqueidentifier
--as
--Begin
--	SELECT distinct(M.Id) AS DISTINCT_MenuID
--	, M.Id
--	, @CurrentUserId as UserId
--	, M.MenuTitle
--	, M.ParentId
--	, M.PageCode
--	, M.ShowMenu
--	, M.CssClass
--	, M.Icon
--	, M.Link
--	, M.Area
--	, M.ControllerName
--	, M.ActionName
--	, CONVERT(bit,0) AS CurentUser_Person_Checkecd
--	, CONVERT(bit,0) AS CurentUser_Group_Checkecd
--	, CONVERT(bit,(SELECT CASE WHEN AM.IsDeleted = 0 THEN 1 WHEN AM.IsDeleted = 1 THEN 0 WHEN AM.IsDeleted IS NULL THEN 0 END)) AS Person_Checkecd
--	, CONVERT(bit,(SELECT CASE WHEN AMG.IsDeleted = 0 THEN 1 WHEN AMG.IsDeleted = 1 THEN 0 WHEN AMG.IsDeleted IS NULL THEN 0 END)) AS Group_Checkecd
--	, CONVERT(bit,(SELECT CASE WHEN AMG.IsDeleted = 0 THEN 1 WHEN AMG.IsDeleted = 1 THEN 0 WHEN AMG.IsDeleted IS NULL THEN 0 END)) AS [disabled]
--	, ISNULL((SELECT COUNT(Id) 
--				FROM dbo.Menus AS MP
--				WHERE (ParentId = M.Id)), 0) AS IsParent
--	FROM dbo.Menus AS M 
--	LEFT OUTER JOIN dbo.UserAccessMenus AS AM ON AM.MenuId=M.Id and AM.UserId=@CurrentUserId
--	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG ON AMG.MenuId=M.Id and AMG.GroupId in (select UG.GroupId 
--																							from GroupUsers as UG
--																							where UG.UserId=@CurrentUserId and UG.IsActive=1 and UG.IsDeleted=0) 
--	WHERE (M.IsDeleted = 0) AND (M.IsActive = 1)
--	order by PageCode,ControllerName
--End
Create PROCEDURE [dbo].[Get_UserCurrentAccess]
    @CurrentUserId uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT 
        M.Id AS DISTINCT_MenuID,
        M.Id,
        @CurrentUserId AS UserId,
        M.MenuTitle,
        M.ParentId,
        M.PageCode,
        M.ShowMenu,
        M.CssClass,
        M.Icon,
        M.Link,
        M.Area,
        M.ControllerName,
        M.ActionName,
        CONVERT(BIT, 0) AS CurentUser_Person_Checkecd,
        CONVERT(BIT, 0) AS CurentUser_Group_Checkecd,
        X.Person_Checkecd,
        X.Group_Checkecd,
        X.Group_Checkecd AS [disabled],
        ISNULL(
            (SELECT COUNT(1)
             FROM dbo.Menus AS MP
             WHERE MP.ParentId = M.Id), 0) AS IsParent
    FROM dbo.Menus AS M
    LEFT JOIN dbo.UserAccessMenus AS AM 
        ON AM.MenuId = M.Id AND AM.UserId = @CurrentUserId
    LEFT JOIN dbo.GroupAccessMenus AS AMG 
        ON AMG.MenuId = M.Id AND AMG.GroupId IN (
            SELECT UG.GroupId
            FROM dbo.GroupUsers AS UG
            WHERE UG.UserId = @CurrentUserId AND UG.IsActive = 1 AND UG.IsDeleted = 0
        )
    CROSS APPLY (
        SELECT
            CONVERT(BIT, CASE WHEN AM.IsDeleted = 0 THEN 1 ELSE 0 END) AS Person_Checkecd,
            CONVERT(BIT, CASE WHEN AMG.IsDeleted = 0 THEN 1 ELSE 0 END) AS Group_Checkecd
    ) AS X
    WHERE 
        M.IsDeleted = 0 AND 
        M.IsActive = 1 AND 
        (X.Person_Checkecd = 1 OR X.Group_Checkecd = 1)
    ORDER BY M.PageCode, M.ControllerName
END

GO
/****** Object:  StoredProcedure [dbo].[Get_UserSelectedAccess_With_CurrentUserAccess_For_TreeCheckBox]    Script Date: 4/23/2024 2:57:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[Get_UserSelectedAccess_With_CurrentUserAccess_For_TreeCheckBox]
@CurrentUserId uniqueidentifier
,@UserSelected_UserId uniqueidentifier
as
Begin
	SELECT distinct(M.Id) AS DISTINCT_MenuID
	, M.Id
	, @CurrentUserId as UserId
	, M.MenuTitle
	, M.ParentId
	, M.PageCode
	, M.ShowMenu
	, M.CssClass
	, M.Icon
	, M.Link
	, M.Area
	, M.ControllerName
	, M.ActionName
	-- لیست منو‌هایی که کاربر جاری به صورت فردی و گروهی به آن دسترسی دارد
	, CONVERT(bit,(SELECT CASE WHEN AM_Current.IsDeleted = 0 THEN 1 WHEN AM_Current.IsDeleted = 1 THEN 0 WHEN AM_Current.IsDeleted IS NULL THEN 0 END)) AS CurentUser_Person_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_User.IsDeleted = 0 THEN 1 WHEN AMG_User.IsDeleted = 1 THEN 0 WHEN AMG_User.IsDeleted IS NULL THEN 0 END)) AS CurentUser_Group_Checkecd
	, ISNULL((SELECT COUNT(Id) 
			FROM dbo.Menus AS MP
			WHERE (ParentId = M.Id)), 0) AS IsParent
	-- لیست منوهایی که کار انتخاب شده به آن به صورت فردی و گروهی دسترسی دراد
	, CONVERT(bit,(SELECT CASE WHEN AM_SelectedUser.IsDeleted = 0 THEN 1 WHEN AM_SelectedUser.IsDeleted = 1 THEN 0 WHEN AM_SelectedUser.IsDeleted IS NULL THEN 0 END)) AS Person_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_UserSelected_Group.IsDeleted = 0 THEN 1 WHEN AMG_UserSelected_Group.IsDeleted = 1 THEN 0 WHEN AMG_UserSelected_Group.IsDeleted IS NULL THEN 0 END)) AS Group_Checkecd
	, CONVERT(bit,(SELECT CASE WHEN AMG_UserSelected_Group.IsDeleted = 0 THEN 1 WHEN AMG_UserSelected_Group.IsDeleted = 1 THEN 0 WHEN AMG_UserSelected_Group.IsDeleted IS NULL THEN 0 END)) AS [disabled]

	FROM dbo.Menus AS M 
	LEFT OUTER JOIN dbo.UserAccessMenus AS AM_Current ON AM_Current.MenuId=M.Id and AM_Current.UserId=@CurrentUserId
	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_User ON AMG_User.MenuId=M.Id and AMG_User.GroupId in (select UG.GroupId 
																							from GroupUsers as UG
																							where UG.UserId=@CurrentUserId and UG.IsActive=1 and UG.IsDeleted=0)
	
	LEFT OUTER JOIN dbo.UserAccessMenus AS AM_SelectedUser ON AM_SelectedUser.MenuId=M.Id and AM_SelectedUser.UserId=@UserSelected_UserId
	LEFT OUTER JOIN dbo.GroupAccessMenus AS AMG_UserSelected_Group ON AMG_UserSelected_Group.MenuId=M.Id and AMG_UserSelected_Group.GroupId in (select UG.GroupId 
																							from GroupUsers as UG
																							where UG.UserId=@UserSelected_UserId and UG.IsActive=1 and UG.IsDeleted=0)
	WHERE (M.IsDeleted = 0) AND (M.IsActive = 1)
	order by PageCode,ControllerName
End
Go





--CREATE VIEW [dbo].[View_UserMenubars]
--AS
--SELECT    M.Id, M.MenuTitle, M.ParentId, M.PageCode, M.ShowMenu, M.CssClass, M.Icon, M.Link,M.Area, M.ControllerName, M.ActionName, CONVERT(bit,
--                          (SELECT    CASE WHEN AM.IsDeleted = 0 THEN 1 WHEN AM.IsDeleted = 1 THEN 0 WHEN AM.IsDeleted IS NULL THEN 0 END AS Expr1)) AS Person_Checkecd, CONVERT(bit,
--                          (SELECT    CASE WHEN AMG.IsDeleted = 0 THEN 1 WHEN AMG.IsDeleted = 1 THEN 0 WHEN AMG.IsDeleted IS NULL THEN 0 END AS Expr1)) AS Group_Checkecd, AM.UserId, ISNULL
--                          ((SELECT    COUNT(Id) AS Expr1
--                              FROM         dbo.Menus AS MP
--                              WHERE     (ParentId = M.Id)), 0) AS IsParent
--FROM         dbo.Menus AS M LEFT OUTER JOIN
--                      dbo.UserAccessMenus AS AM ON AM.MenuId = M.Id LEFT OUTER JOIN
--                      dbo.GroupAccessMenus AS AMG ON AMG.MenuId = M.Id
--WHERE     (M.IsDeleted = 0) AND (M.IsActive = 1)
--GO
