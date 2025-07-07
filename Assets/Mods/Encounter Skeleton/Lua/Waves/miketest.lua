-- The bouncing bullets attack from the documentation example.
spawntimer = 0
options = {}
text = {}
miclen = #Mike.getMicrophones


    if GetAlMightyGlobal("MikeChoice") then
        Mike.setMicrophone(GetAlMightyGlobal("MikeChoice"), false)
    end

    if GetAlMightyGlobal("MikeSense") then
        Mike.sensitivity = (GetAlMightyGlobal("MikeSense"))
    end

    local t = CreateText({"Microphone"}, {Arena.x+20,(Arena.height+Arena.y) -20}, 560)
    t.HideBubble()
    t.color = {1,1,1}
    t.SetAutoWaitTimeBetweenTexts(0)
    t.progressmode = "None"
    t.SetVoice("")
    table.insert(text, t)

    
    local t = CreateText({"Sensitivity"}, {Arena.x-120,(Arena.height+Arena.y) -20}, 560)
    t.HideBubble()
    t.color = {1,1,1}
    t.SetAutoWaitTimeBetweenTexts(0)
    t.progressmode = "None"
    t.SetVoice("")
    table.insert(text, t)



    
    local t = CreateText({"Done"}, {Arena.x-30,(100)}, 560)
    t.HideBubble()
    t.color = {1,1,1}
    t.SetAutoWaitTimeBetweenTexts(0)
    t.progressmode = "None"
    t.SetVoice("")
    table.insert(text, t)

    local option = CreateProjectile("empty",0, -Arena.height/2+20)
    option.sprite.Scale(50,1)
    option["confirm"] = true
    option.sprite.alpha = 0
    
    Mike.startRecording()
    
    for i, v in ipairs(Mike.getMicrophones) do
        
        local t = CreateText({string.sub( v:gsub("Microphone", ""),0,10).. "..."}, {Arena.x+10,(Arena.height+Arena.y) -25-20*i}, 560)
        t.HideBubble()
        t.color = {1,1,1}
        t.SetAutoWaitTimeBetweenTexts(0)
        t.progressmode = "None"
        t.SetVoice("")
        table.insert(options, t)
        if (GetAlMightyGlobal("MikeChoice") or 0) == i-1  then
            t.color = {1,1,0}
        end
        table.insert(text, t)

    local option = CreateProjectile("empty",100, Arena.height-90-20*i)
    option.sprite.Scale(150,1)
    option["index"] = i
    option.sprite.alpha = 0


end


    Mike.setVolume(7)
    local volumeImg = CreateProjectile("bar",-80, 10)
    local bar = ((CreateProjectile("empty",0, 0)))
    bar.sprite.Scale(110,5)
    bar.sprite.color={1,1,0}
    volumeImg.sprite.Scale(3,3)

    local testingBlock = ((CreateProjectile("empty",-30, 0)))

function Update()
    
    testingBlock.MoveTo(-Arena.width/2-30, (((Mike.testingVal-2.5)/2.5*Arena.height/2)+(-Arena.height/2))/2)
    testingBlock.sprite.Scale(20, Mike.testingVal/2.5*Arena.height/2)

    if(Mike.testingVal > Mike.sensitivity) then
        testingBlock.sprite.color = {0,0.8,0}
    else
        testingBlock.sprite.color = {1,1,1}
    end

    -- Player.MoveTo(0,Mike.currentNote/2-100)
    if(Player.x < -50) then
        Mike.sensitivity = ((Player.y/Arena.height*2)*2.5+2.5)
        SetAlMightyGlobal("MikeSense",Mike.sensitivity)
    end
    bar.MoveTo(-100,(Mike.sensitivity-2.5)/2.5*Arena.height/2)
end

function OnHit(bullet)
    if(bullet["confirm"]) then
        Mike.stopRecording()
        for i, v in ipairs(text) do
            v.Remove()
        end
        EndWave()
    end
    if(bullet["index"]) then
        for i, v in ipairs(options) do

            if(i == bullet["index"]) then
                v.color = {1,1,0}
                SetAlMightyGlobal("MikeChoice",i-1)
                Mike.setMicrophone(i-1, true)
            else
                v.color = {1,1,1}
            end

        end
    end
end