x = 0
y = 0
function Update()
    if GetInput("Horizontal") > 0 then
        x = x + 1
    end
    if GetInput("Horizontal") < 0 then
        x = x - 1
    end
    spr(x, y)

    if r() then
        restart()
    end
end