function createClass(obj)
	function obj:new()
    	local o={}
    	setmetatable(o, self)
    	self.__index=self
    	o:construct()
    	return o
	end

	return obj
end

function clearTable(t)
	for k, v in pairs(t) do
		t[k]=nil
	end
end

local BotInfoApi=nil
--=====================================TeamApi=======================================

local TeamApi={Count=nil}

function TeamApi:construct()
    self.Instances={}
    self.Units={}
    self.Flags={}
end

function TeamApi:receiveUnitInfo()
	clearTable(self.Units)
    for k=1, self.Count do
    	local file=self.Instances[k].File
    	file:seek("set", 0)
        for line in file:lines() do
            local properties, i={}, 1
            for p in string.gmatch(line, "([^%s]+)") do
                properties[i]=p
                i=i+1
            end
            self.Units[#self.Units+1]={id=properties[1], class=properties[2], name=properties[3], cost=properties[4], flag=properties[5]}
        end
    end
end

function TeamApi:closeFiles()
	for i=1, #self.Instances do
		self.Instances[i].File:close()
	end
end

function haveUnit(units, property, count, ...)
    if property=="class" or property=="name" then
    	for i, unit in pairs(units) do
        	for j, parameter in pairs(arg) do
            	if unit[property]:find(parameter) then
            		count=count-1
            		if count==0 then
            			return true
            		end
            	end
        	end
    	end
    else
    	for i, unit in pairs(units) do
        	for j, parameter in pairs(arg) do
            	if unit[property]>=parameter then
            		count=count-1
            		if count==0 then
            			return true
            		end
            	end
        	end
    	end
	end
    return false
end

TeamApi=createClass(TeamApi)
local EnemyTeam=TeamApi:new()
local MyTeam=TeamApi:new()

function EnemyTeam:getFlagUnits(flags)
	for i, unit in pairs(self.Units) do
		if unit.flag~="n" then
			--table.insert(flags[unit.flag].squads_enemy.units, unit)	-- For possible future use.
			flags[unit.flag].count_squads_enemy=flags[unit.flag].count_squads_enemy+1
		end
	end
end

function MyTeam:getFlagUnits(flags, units)
	for i, unit in pairs(units) do
		if unit.flag~="n" then
			--table.insert(flags[unit.flag].squads_team.units, unit)
			flags[unit.flag].count_squads_team=flags[unit.flag].count_squads_team+1
		end
	end
end

--=====================================ContextApi=======================================

local Context={
    Instance={File=nil, Id=nil, Army=nil, SpecialPoints=15},
	SpawnInfo=nil,
	SpawnBuffer={units={}, count=1, pointer=1},
	TimedUnits={},
	SceneUnits={},
	Flags={points={}, captured=nil, enemy=nil, neutral=nil, count=0, total_rate=1},
	Utility={FilePath=nil}
}

function Context:setGroupTimer(group, wait_time)
	BotInfoApi.Players.Me.TimedUnits[group]=BotApi.Events:SetQuantTimer(function() BotInfoApi.Players.Me.TimedUnits[group]=nil end, wait_time*1000)
end

function Context:addSceneUnit(id, unit)
	if unit==nil then
		return false
	end

	if unit.charge>80 and BotInfoApi.Options.SpecialMode==false then
		BotInfoApi.Players.Me:setGroupTimer(unit.group, unit.charge)
	end
	self.SceneUnits[id]={class=unit.class, name=unit.name, cost=unit.cost, flag=unit.flag, timer=unit.timer} -- self.SceneUnits[id]=unit creates unit's member values as references!

	return true
end

function Context:sendSceneUnits()
	self.Instance.File:close()
	self.Instance.File=io.open(self.Utility.FilePath, "w")
	self.Instance.File:setvbuf("no")

	for id, unit in pairs(self.SceneUnits) do
		if BotApi.Scene:IsSquadExists(id) then
			self.Instance.File:write(id, " ", unit.class, " ", unit.name, " ", unit.cost, " ", unit.flag, "\n")
		else
			if unit.timer then
				BotApi.Events:KillQuantTimer(unit.timer)
			end
			self.SceneUnits[id]=nil
		end
	end
end

function Context:initializeFlags()
	for i, flag in pairs(BotApi.Scene.Flags) do
		self.Flags.points[flag.name]={}
		self.Flags.points[flag.name].occupant=nil
		self.Flags.points[flag.name].status=nil
		self.Flags.points[flag.name].priority=nil
		self.Flags.points[flag.name].count_squads_enemy=nil		--self.Flags.points[flag.name].squads_enemy={units={}, count=nil}	-- For possible future use.
		self.Flags.points[flag.name].count_squads_team=nil		--self.Flags.points[flag.name].squads_team={units={}, count=nil}
		self.Flags.count=self.Flags.count+1
	end
end

local FlagStatus = {
	Clear="clear",
	Defended="def",
	DefendedStrong="def_strong",
	Attacked="atk",
	AttackedStrong="atk_strong"
}
function Context:updateFlagPriorities()
	local team_my=BotApi.Instance.team
	local team_enemy=BotApi.Instance.enemyTeam
	
	for name, flag in pairs(self.Flags.points) do
		--clearTable(flag.squads_enemy.units)
		--clearTable(flag.squads_team.units)
		flag.count_squads_enemy=0
		flag.count_squads_team=0
	end

	BotInfoApi.Players.Enemy:getFlagUnits(self.Flags.points)
	BotInfoApi.Players.Team:getFlagUnits(self.Flags.points, BotInfoApi.Players.Team.Units)
	BotInfoApi.Players.Team:getFlagUnits(self.Flags.points, self.SceneUnits)

	self.Flags.total_rate=1
	self.Flags.captured=0; self.Flags.enemy=0; self.Flags.neutral=0
	for i, flag in pairs(BotApi.Scene.Flags) do
		local ratio=self.Flags.points[flag.name].count_squads_enemy-self.Flags.points[flag.name].count_squads_team
		if flag.occupant == team_my then self.Flags.captured = self.Flags.captured+1
			self.Flags.points[flag.name].priority=5
			if ratio>2 then
				self.Flags.points[flag.name].status=FlagStatus.AttackedStrong
				self.Flags.points[flag.name].priority=self.Flags.points[flag.name].priority+10
			elseif ratio>0 then
				self.Flags.points[flag.name].status=FlagStatus.Attacked
				self.Flags.points[flag.name].priority=self.Flags.points[flag.name].priority+5
			elseif ratio<0 then
				self.Flags.points[flag.name].priority=self.Flags.points[flag.name].priority+ratio
				self.Flags.points[flag.name].status=FlagStatus.Defended
			else
				self.Flags.points[flag.name].status=FlagStatus.Clear
			end
		elseif flag.occupant == team_enemy then self.Flags.enemy = self.Flags.enemy+1
			self.Flags.points[flag.name].priority=10
			if ratio>2 then
				self.Flags.points[flag.name].status=FlagStatus.DefendedStrong
				self.Flags.points[flag.name].priority=self.Flags.points[flag.name].priority-ratio
			elseif ratio>0 then
				self.Flags.points[flag.name].status=FlagStatus.Defended
				self.Flags.points[flag.name].priority=self.Flags.points[flag.name].priority
			elseif self.Flags.points[flag.name].count_squads_team>1 then
				self.Flags.points[flag.name].priority=self.Flags.points[flag.name].priority+10
				self.Flags.points[flag.name].status=FlagStatus.Attacked
			else
				self.Flags.points[flag.name].status=FlagStatus.Clear
			end
		else self.Flags.neutral = self.Flags.neutral+1
			self.Flags.points[flag.name].priority=10
			if ratio~=0 then
				self.Flags.points[flag.name].status=FlagStatus.Defended
				self.Flags.points[flag.name].priority=self.Flags.points[flag.name].priority-ratio*2
			else
				self.Flags.points[flag.name].status=FlagStatus.Clear
				self.Flags.points[flag.name].priority=self.Flags.points[flag.name].priority*2
			end
		end
		if self.Flags.points[flag.name].priority<1 then
			self.Flags.points[flag.name].priority=1
		end
		self.Flags.total_rate=self.Flags.total_rate+self.Flags.points[flag.name].priority
		self.Flags.points[flag.name].occupant=flag.occupant
	end
end

--=====================================GameMode=======================================

local GameMode = {
	SpecialPoints=nil,
	priority_default=function(priority, factors) return priority end,
	order_default	=function(id) orderCaptureFlag(id, getPriorityFlag(BotInfoApi.Players.Me.Flags.points, BotInfoApi.Players.Me.Flags.total_rate), 120000) end -- 2 min; 1000 tic == 1 sec
}

function GameMode:construct()
    self.Purchases={}
    self.Factors={}
end

function GameMode:initialize()
end
function GameMode:calculateFactors()
end

GameMode=createClass(GameMode)
local AssaultZones=GameMode:new()
local Liquidation=GameMode:new()

function AssaultZones:initialize()
	self.SpecialPoints=15
	self.Purchases = {
		inf_com 	= {units={}, priority=10, limit=-1,	["getCurrentPriority"]=	function(priority)
		 																	if self.Factors["has_ainf"] then
		 																		priority=priority-2
																			end
																			if not self.Factors["have_enough_inf"] then
																				priority=priority*20
																			end
																			return priority
																		end,
														["setOrder"]=function(id) orderCaptureFlag(id, getSpecialFlag(nil, {[FlagStatus.Clear]={flags={}, total_rate=0}}), 150000) end},
		inf_tac 	= {units={}, priority=10, limit=-1,	["getCurrentPriority"]=nil,						["setOrder"]=function(id) orderCaptureFlag(id, getSpecialFlag(BotApi.Instance.enemyTeam, {[FlagStatus.Clear]={flags={}, total_rate=0}}), 150000) end},
		inf_vet		= {units={}, priority=8,  limit=-1, ["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_sab	 	= {units={}, priority=3,  limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=function(id) orderCaptureFlag(id, getSpecialFlag(BotApi.Instance.enemyTeam, {[FlagStatus.Clear]={flags={}, total_rate=0}, [FlagStatus.Defended]={flags={}, total_rate=0}}), 130000) end},
		inf_def	 	= {units={}, priority=7,  limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=function(id) orderCaptureFlag(id, getSpecialFlag(BotApi.Instance.team, {[FlagStatus.Clear]={flags={}, total_rate=0}, [FlagStatus.Defended]={flags={}, total_rate=0}}), 240000) end},
		inf_bld		= {units={}, priority=1,  limit=2,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=function(id) orderCaptureFlag(id, getSpecialFlag(BotApi.Instance.team, {[FlagStatus.DefendedStrong]={flags={}, total_rate=0}, [FlagStatus.Defended]={flags={}, total_rate=0}}), 210000) end},
		inf_a_inf 	= {units={}, priority=8,  limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_a_veh 	= {units={}, priority=6,  limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_a_all 	= {units={}, priority=8,  limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},

		
		sup_sct  	= {units={}, priority=3, limit=-1,	["getCurrentPriority"]=	function(priority)
		 												   					if BotInfoApi.Players.Me.Flags.neutral>0 then
		 												  	    				priority=priority+2
		 												  					end
		 												  					return priority
																		end,
														["setOrder"]=function(id) orderCaptureFlag(id, getSpecialFlag(nil, {[FlagStatus.Clear]={flags={}, total_rate=0}, [FlagStatus.Defended]={flags={}, total_rate=0}}), 80000) end},
		sup_a_inf 	= {units={}, priority=3, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		sup_a_veh 	= {units={}, priority=3, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		sup_a_all 	= {units={}, priority=4, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},

		veh_art  	= {units={}, priority=2, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=function(id) orderSpecial(id, 3600000) end},
		veh_a_inf 	= {units={}, priority=4, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		veh_a_veh 	= {units={}, priority=3, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		veh_a_all 	= {units={}, priority=5, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=function(id) orderCaptureFlag(id, getSpecialFlag(BotApi.Instance.enemyTeam, {[FlagStatus.Defended]={flags={}, total_rate=0}}), 120000) end},

		par    	 	= {units={}, priority=6, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=nil},
		strike 	 	= {units={}, priority=2, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},

		ult    	 	= {units={}, priority=2, limit=-1,	["getCurrentPriority"]=	function(priority)
																			if BotInfoApi.Players.Me.Flags.enemy==0 then
																				priority=priority-2
																			elseif BotInfoApi.Players.Me.Flags.captured<BotInfoApi.Players.Me.Flags.enemy then
																				priority=priority+2
																			end
																			return priority
		 																end,
		 												["setOrder"]=function(id) orderCaptureFlag(id, getSpecialFlag(BotApi.Instance.enemyTeam, {[FlagStatus.DefendedStrong]={flags={}, total_rate=0}, [FlagStatus.Defended]={flags={}, total_rate=0}}), 120000) end},

		free_inf_com	= {units={}, priority=10, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		free_inf_a_veh 	= {units={}, priority=6,  limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		free_veh_a_all  = {units={}, priority=2,  limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default}
	}
	self.Purchases.inf_tac["getCurrentPriority"]=self.Purchases.inf_com.getCurrentPriority
	self.Purchases.par["setOrder"]=self.Purchases.inf_sab.setOrder
end


function AssaultZones:calculateFactors()
	self.Factors["have_enough_inf"]		=haveUnit(BotInfoApi.Players.Me.SceneUnits, "class", 4, "inf_") 		-- Some examples here. Available criterias: class, name, cost.
	self.Factors["have_enough_free_inf"]=haveUnit(BotInfoApi.Players.Me.SceneUnits, "class", 2, "free_inf")
	self.Factors["have_free_veh"] 	 	=haveUnit(BotInfoApi.Players.Me.SceneUnits, "class", 1, "free_veh")

	self.Factors["has_ainf"]		 	=haveUnit(BotInfoApi.Players.Enemy.Units,"class", 4, "inf_a_inf", "sup_a_Inf", "veh_a_inf")
	--self.Factors["has_tanks"]	 	  	=haveUnit(BotInfoApi.Players.Enemy.Units, "class", 1, veh_a_inf)
	--self.Factors["has_hellhound"] 	=haveUnit(BotInfoApi.Players.Enemy.Units, "name", 1, "hellhound")

	--self.Factors["has_dreadnought"] 	=haveUnit(BotInfoApi.Players.Team.Units, "name", 2, "dreadnought")
	--self.Factors["has_expensive_unit"]=haveUnit(BotInfoApi.Players.Team.Units, "cost", 1, 1000)				-- Finds units in this instance's team with cost >= 1000
end

function Liquidation:initialize()
	self.SpecialPoints=0
	self.Purchases = {
		inf_com 	= {units={}, priority=1, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_tac 	= {units={}, priority=3, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_vet		= {units={}, priority=3, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_sab	 	= {units={}, priority=2, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_def	 	= {units={}, priority=3, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_bld		= {units={}, priority=1, limit=2,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_a_inf 	= {units={}, priority=4, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_a_veh 	= {units={}, priority=4, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		inf_a_all 	= {units={}, priority=7, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},

		sup_sct  	= {units={}, priority=3, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		sup_a_inf 	= {units={}, priority=4, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		sup_a_veh 	= {units={}, priority=4, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		sup_a_all 	= {units={}, priority=4, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},

		veh_art  	= {units={}, priority=3, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=function(id) orderSpecial(id, 3600000) end},
		veh_a_inf 	= {units={}, priority=4, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		veh_a_veh 	= {units={}, priority=6, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},
		veh_a_all 	= {units={}, priority=7, limit=-1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default},

		ult    	 	= {units={}, priority=8, limit=1,	["getCurrentPriority"]=self.priority_default,	["setOrder"]=self.order_default}
	}
end

function Liquidation:calculateFactors()
	-- Nothing is here yet.
end

--=====================================BotInfoApi=======================================

BotInfoApi = {
	Path="mods\\"..ModFolderName.."\\resource\\script\\multiplayer\\bot_info\\",
    Players={Enemy=EnemyTeam, Team=MyTeam, Me=Context, Count=nil},
    Options={SpecialMode=false, GameMode=nil, SpawnDelay=1}
}

function BotInfoApi:initialize()
	local team_my=BotApi.Instance.team
	local team_enemy=BotApi.Instance.enemyTeam
	local id_my=BotApi.Instance.playerId
	local army_my=BotApi.Instance.army
	local dir_content=io.popen("dir \""..self.Path.."\" /b")
	local income=BotApi.Commands:Income(id_my)
	if income==1 then
		self.Players.Me.Instance.SpecialPoints=1000
		self.Options.SpecialMode=true
	end

	self.Players.Me.Utility.FilePath=self.Path..team_my..tostring(id_my)..army_my
	self.Players.Me.Instance.File=io.open(self.Players.Me.Utility.FilePath, "w")
	self.Players.Me.Instance.Id=id_my
	self.Players.Me.Instance.Army=BotApi.Instance.army
	self.Players.Me:initializeFlags()
	self.Players.Me:updateFlagPriorities()

	if self.Players.Me.Flags.count==1 and income<1 then
		self.Options.SpecialMode=true
		self.Options.GameMode=Liquidation
		self.Options.SpawnDelay=100
	else
		self.Options.GameMode=AssaultZones
	end
	self.Options.GameMode:initialize()
	self.Players.Me.Instance.SpecialPoints=self.Options.GameMode.SpecialPoints

	local id_tmp
	local id_pattern="%d+"
	local army_pattern="%a%a%w+"
	team_my=team_my..id_pattern
	team_enemy=team_enemy..id_pattern
	for filename in dir_content:lines() do
		id_tmp=tonumber(filename:match(id_pattern))
    	if id_tmp==id_my then
    	-- Do nothing.
    	elseif filename:match(team_enemy) then
    		self.Players.Enemy.Instances[#self.Players.Enemy.Instances+1]={File=io.open(self.Path..filename, "r"), Id=id_tmp, Army=filename:match(army_pattern)}
    	elseif filename:match(team_my) then
			self.Players.Team.Instances[#self.Players.Team.Instances+1]={File=io.open(self.Path..filename, "r"), Id=id_tmp, Army=filename:match(army_pattern)}
    	end
	end
	dir_content:close()
	self.Players.Enemy.Count=#self.Players.Enemy.Instances
	self.Players.Team.Count=#self.Players.Team.Instances
	self.Players.Count=self.Players.Enemy.Count+self.Players.Team.Count+1
end

function BotInfoApi:terminate()
	self.Players.Me.Instance.File:close()
	self.Players.Team:closeFiles()
	self.Players.Enemy:closeFiles()
end

--======================================================================================

function onScriptInit()
	os.execute("del \""..BotInfoApi.Path.."\"* /q")
	io.open(BotInfoApi.Path..BotApi.Instance.team..tostring(BotApi.Instance.playerId)..BotApi.Instance.army, "w+"):close()
end

function onScriptDone()
	BotInfoApi:terminate()
	collectgarbage("collect")
end

function readSetFile(fname, army)
	local file=io.open(fname, "r")
	local count=0

    if file~=nil then
        while true do
            local line=file:read("*l")
            if line==nil then break end

            line=line:gsub(";.*",""):gsub("^%s*(.-)%s*$", "%1")
            local class=line:match("t[(].+%s([%a%_]+)[)]")
            if line:lower():find("[(]"..army.."[)]") and line:len()>0 and BotInfoApi.Options.GameMode.Purchases[class]~=nil then
                local name=line:gsub( ".*name[(]", ""):gsub( "[)].*", "")
                if name:len()>0 then
                	local group  =line:match(" g[(]([^)]*)[)]")
                	local charge =tonumber(line:match("%s+c[(]([^)]*)[)]"))
                	local cost	 =line:gsub( ".*[{]cost ", ""):gsub( ".*cost[(]", ""):gsub( "[})].*", ""); cost=tonumber(cost); if cost==nil then cost=200 end
                	local fore 	 =1
                    if not name:match("{") then
                    	fore=fore-tonumber(line:match(" f[(]([^)]*)[)]"))
                    	count=count+1
						BotInfoApi.Options.GameMode.Purchases[class].units[count]={name=name.."("..army..")", cost=cost, charge=charge, group=group}
						if BotInfoApi.Players.Me.TimedUnits[group]==nil and not BotInfoApi.Options.SpecialMode then
                            BotInfoApi.Players.Me:setGroupTimer(group, charge*fore)
                        end
                    else
                        name=line:gsub( "{\"", ""):gsub( "\".*", "")
                        if not name:match("mp/") then
                        	fore=fore-tonumber(line:match("[{]fore ([^)]*)[}}][}}]"))
                        	count=count+1
							BotInfoApi.Options.GameMode.Purchases[class].units[count]={name=name, cost=cost, charge=charge, group=group}
							if BotInfoApi.Players.Me.TimedUnits[group]==nil and not BotInfoApi.Options.SpecialMode then
                            	BotInfoApi.Players.Me:setGroupTimer(group, charge*fore)
                        	end
                        end
                    end
                end
            end
        end
        print("Units read in", fname, ":", count)
        io.close(file)
    end
end

function getPriorityFlag(flags, total_rate)
	local rnd=math.floor(math.random()*total_rate)
	local selected_flag=nil

	for name, flag in pairs(flags) do
		rnd=rnd-flag.priority
		if rnd<1 then
			return name
		end
	end

	return getPriorityFlag(flags, total_rate)	-- Unreachable code in normal conditions. Flag names on badly made maps can make collisions in table.
end

function getSpecialFlag(occupant, statuses)
	for name, flag in pairs(BotInfoApi.Players.Me.Flags.points) do
		if flag.occupant==occupant then
			if statuses[flag.status]~=nil then
				statuses[flag.status].total_rate=statuses[flag.status].total_rate+flag.priority
				statuses[flag.status].flags[name]=flag
			end
		end
	end
	for status, content in pairs(statuses) do
		if content.total_rate>0 then
			return getPriorityFlag(content.flags, content.total_rate)
		end
	end

	return getPriorityFlag(BotInfoApi.Players.Me.Flags.points, BotInfoApi.Players.Me.Flags.total_rate)
end

function orderSpecial(id, delay)
	BotInfoApi.Players.Me.SceneUnits[id].flag="n"
	BotInfoApi.Players.Me.SceneUnits[id].timer=BotApi.Events:SetQuantTimer(function() BotInfoApi.Players.Me.SceneUnits[id].timer=nil end, delay)
end

function orderCaptureFlag(id, flag, delay)
	BotApi.Commands:CaptureFlag(id, flag)
	BotInfoApi.Players.Me.SceneUnits[id].flag=flag
	BotInfoApi.Players.Me.SceneUnits[id].timer=BotApi.Events:SetQuantTimer(function() BotInfoApi.Players.Me.SceneUnits[id].timer=nil end, delay)
end

function selectRandomUnit(available_units, total_rate)
	local rnd=math.floor(math.random()*total_rate)
	local selected_class=nil

	for class, content in pairs(available_units) do
		rnd=rnd-content.rate
		if rnd<1 then
			selected_class=class
			break
		end
	end

	local t=available_units[selected_class].units[math.random(#available_units[selected_class].units)]
	t.class=selected_class
	t.flag=nil
	t.timer=nil

	return t
end

function getUnitToSpawn()
	BotInfoApi.Players.Enemy:receiveUnitInfo()
	BotInfoApi.Players.Team:receiveUnitInfo()
	BotInfoApi.Options.GameMode:calculateFactors()

	local team_size, income, formula=BotApi.Instance.teamSize, nil, nil 		-- Some optimisation here.

	if not BotInfoApi.Options.SpecialMode then
		income=BotApi.Commands:Income(BotApi.Instance.playerId)
		formula=(374*income-31.3*income*income+1.1*income*income*income-1.3)+(354.5*team_size-23*team_size*team_size-342)
	else
		formula=100000
	end

	local total_class_rate, available_class_count, available_units=1, 0, {}
	for class, content in pairs(BotInfoApi.Options.GameMode.Purchases) do
		if content.limit~=0 then
			local unit_count=0
			local current_class={units={}, rate=0, count=0}
			for k, unit in pairs(content.units) do
				if formula>=unit.cost and not (BotInfoApi.Players.Me.TimedUnits[unit.group]~=nil or (unit.cost<10 and unit.cost>BotInfoApi.Players.Me.Instance.SpecialPoints)) then
					unit_count=unit_count+1
					current_class.units[unit_count]=unit
				end
			end

			if #current_class.units>0 then
				available_class_count=available_class_count+1
				available_units[class]=current_class
				available_units[class].rate=content["getCurrentPriority"](content.priority)
				total_class_rate=total_class_rate+available_units[class].rate
			end
		end
	end

	if available_class_count==0 then
		return nil
	end

	local selected_unit=selectRandomUnit(available_units, total_class_rate)
	if selected_unit.charge>80 and not BotInfoApi.Options.SpecialMode then
		BotInfoApi.Players.Me.TimedUnits[selected_unit.group]=5 			-- Disabling this group to be purchased right again.
	end
	if selected_unit.cost<10 then
		BotInfoApi.Players.Me.Instance.SpecialPoints=BotInfoApi.Players.Me.Instance.SpecialPoints-selected_unit.cost
	end
	if BotInfoApi.Options.GameMode.Purchases[selected_unit.class].limit>0 then
		BotInfoApi.Options.GameMode.Purchases[selected_unit.class].limit=BotInfoApi.Options.GameMode.Purchases[selected_unit.class].limit-1
	end

	BotInfoApi.Players.Me.SpawnBuffer.count=BotInfoApi.Players.Me.SpawnBuffer.count+1
	BotInfoApi.Players.Me.SpawnBuffer.units[BotInfoApi.Players.Me.SpawnBuffer.count]=selected_unit

	return selected_unit
end

function onGameStart()
	local army=BotApi.Instance.army
	Quants=0

	math.randomseed(os.clock()*10000000000*BotApi.Instance.playerId*BotApi.Instance.hostId)
	BotInfoApi:initialize()
	readAllUnits(army)

	local marker={class="inf_com", name="bot_marker("..army..")", flag=nil, timer=nil, group="bot_marker", cost=1, charge=1}
	BotInfoApi.Players.Me.SpawnInfo=marker
	BotInfoApi.Players.Me.SpawnBuffer.units[BotInfoApi.Players.Me.SpawnBuffer.count]=marker
end

function onGameQuant()
	Quants=Quants+1
	if Quants%150==0 then
		BotInfoApi.Players.Me:updateFlagPriorities()
		for i, id in pairs(BotApi.Scene.Squads) do
			if BotInfoApi.Players.Me.SceneUnits[id]==nil then
				BotInfoApi.Players.Me.SceneUnits[id]={class="inf_tac", name="sq_special", cost=300, flag=nil, timer=nil}
			end
			if BotInfoApi.Players.Me.SceneUnits[id].timer==nil then
				BotInfoApi.Options.GameMode.Purchases[BotInfoApi.Players.Me.SceneUnits[id].class]["setOrder"](id)
			end
		end
		BotInfoApi.Players.Me:sendSceneUnits()
	end

	if BotInfoApi.Players.Me.SpawnInfo==nil or BotApi.Commands:Spawn(BotInfoApi.Players.Me.SpawnInfo.name, 10) then
		BotInfoApi.Players.Me.SpawnInfo=nil
		if Quants%BotInfoApi.Options.SpawnDelay==0 then
			BotInfoApi.Players.Me.SpawnInfo=getUnitToSpawn()
		end
	end
end

function onGameSpawn(args)
	if BotInfoApi.Players.Me:addSceneUnit(args.squadId, BotInfoApi.Players.Me.SpawnBuffer.units[BotInfoApi.Players.Me.SpawnBuffer.pointer]) then
		BotInfoApi.Players.Me.SpawnBuffer.units[BotInfoApi.Players.Me.SpawnBuffer.pointer]=nil
		BotInfoApi.Players.Me.SpawnBuffer.pointer=BotInfoApi.Players.Me.SpawnBuffer.pointer+1
		BotInfoApi.Options.GameMode.Purchases[BotInfoApi.Players.Me.SceneUnits[args.squadId].class]["setOrder"](args.squadId)
	end
end

function onGameEnd()
	local phrases={on_victory={"gg", "ez", "We won!", "Enemy team sucks!", "Rock 'N Stone, Brothers!", "Haha, losers", ":D"},
	 			   on_defeat={"This sucks", "My teammates are noobs", "Freeman you fool!", "...", "I'm out", "Rematch!", ":("}}
	
	if math.random(7)>4 then
		if BotInfoApi.Players.Me.Flags.captured>BotInfoApi.Players.Me.Flags.enemy then
			BotApi.Commands:SayChat(phrases.on_victory[math.random(#phrases.on_victory)])
		else
			BotApi.Commands:SayChat(phrases.on_defeat[math.random(#phrases.on_defeat)])
		end
	end

	for id, unit in pairs(BotInfoApi.Players.Me.SceneUnits) do
		if unit.timer then
			BotApi.Events:KillQuantTimer(unit.timer)
		end
	end
end

BotApi.Events:Subscribe(BotApi.Events.Init, onScriptInit)
BotApi.Events:Subscribe(BotApi.Events.GameStart, onGameStart)
BotApi.Events:Subscribe(BotApi.Events.Quant, onGameQuant)
BotApi.Events:Subscribe(BotApi.Events.GameSpawn, onGameSpawn)
BotApi.Events:Subscribe(BotApi.Events.GameEnd, onGameEnd)
BotApi.Events:Subscribe(BotApi.Events.Done, onScriptDone)
