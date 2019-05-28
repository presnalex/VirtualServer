
drop sequence VirtualServerIdSeq;
create sequence VirtualServerIdSeq as int start with 1 increment by 1;

drop table VirtualServer;
create table VirtualServer(
VirtualServerId int,
CreateDateTime datetime2(0) NOT NULL,
RemoveDateTime datetime2(0)
CONSTRAINT [PK_VirtualServer] PRIMARY KEY 
(
	VirtualServerId ASC
)
);

grant select on VirtualServer to atc;

drop procedure [dbo].[pVirtualServerDel];
create procedure [dbo].[pVirtualServerDel] 
	@VirtualServerId int,
	@RemoveDateTime datetime2(0)
as
begin
	update dbo.VirtualServer
	set RemoveDateTime = @RemoveDateTime
	where VirtualServerId = @VirtualServerId;

end;

grant execute on pVirtualServerDel to atc;

create procedure [dbo].[pVirtualServerCreate] 
	@CreateDateTime datetime2(0)
as
begin
	declare @sq int;
	set @sq = next value for VirtualServerIdSeq;
	insert into dbo.VirtualServer
	select @sq, @CreateDateTime, null;

end;


grant execute on pVirtualServerCreate to atc;

