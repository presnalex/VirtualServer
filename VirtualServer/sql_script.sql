-- Скрипт для схемы atc, если схема другая, необходимо поменять схему на актуальную
--drop sequence VirtualServerIdSeq;
create sequence VirtualServerIdSeq as int start with 1 increment by 1;

--drop table VirtualServer;
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

create table VirtualServerUsage (
	ValidFrom datetime2(0),
	ValidTo datetime2(0)
);

grant select on VirtualServerUsage to atc;

--drop procedure [dbo].[pVirtualServerDel];
create procedure [dbo].[pVirtualServerDel] 
	@VirtualServerId int,
	@RemoveDateTime datetime2(0)
as
begin
	update dbo.VirtualServer
	set RemoveDateTime = @RemoveDateTime
	where VirtualServerId = @VirtualServerId;

	declare @ActiveServers int;
	select @ActiveServers = count(1)
	from dbo.VirtualServer
	where RemoveDateTime is null;

	if @ActiveServers = 0
	update VirtualServerUsage
		set ValidTo = @RemoveDateTime
	where ValidTo is null;

end;

grant execute on pVirtualServerDel to atc;

--drop procedure [dbo].[pVirtualServerCreate] ;
create procedure [dbo].[pVirtualServerCreate] 
	@CreateDateTime datetime2(0)
as
begin
	declare @sq int;
	set @sq = next value for VirtualServerIdSeq;
	insert into dbo.VirtualServer
	select @sq, @CreateDateTime, null;

	declare @InUsage int;
	select @InUsage = count(1)
	from VirtualServerUsage
	where ValidTo is null;

	if @InUsage = 0
	insert into VirtualServerUsage
	select @CreateDateTime, null;
end;


grant execute on pVirtualServerCreate to atc;

--drop procedure [dbo].[pVirtualServerGetUsage] ;
create procedure [dbo].[pVirtualServerGetUsage] 
	@CurrDateTime datetime2(0)
as
begin
	select sum(DATEDIFF(second, ValidFrom, coalesce(ValidTo, @CurrDateTime))) as UsageSecondsFromDB
	from VirtualServerUsage t;
end;

grant execute on pVirtualServerGetUsage to atc;