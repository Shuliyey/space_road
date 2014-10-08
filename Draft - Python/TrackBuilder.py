import math
import sys

GRIDSIZE = 24
WIDTH = 1
POINTS_PER_CURVE = 32
grid = [['_' for x in range(GRIDSIZE)] for x in range(GRIDSIZE)]
mid = GRIDSIZE/2
position = [mid,0]
direction = 0

def main():
    #Testing Ground
    line(5)
    curve(5,math.pi, 1)

    end()

def curve(radius, period, turn):
    #Creates an arc based on a radius and period
    global direction
    period_divisions = period/POINTS_PER_CURVE
    xprev = radius *(1-math.cos(direction))
    yprev = radius * math.sin(direction)
    print("NEW")
    for x in range(POINTS_PER_CURVE):
        try:
            grid[int(position[0])][int(position[1])] = 'O'
            grid[int(position[0] + math.cos(direction))][int(position[1] - math.sin(direction))] = '#'
            grid[int(position[0] - math.cos(direction))][int(position[1] + math.sin(direction))] = '#'
            direction = direction + period_divisions
            xdif = radius * (1 - math.cos(direction))
            ydif = radius * math.sin(direction)
            print(xdif)
            position[0] = position[0] + (xdif - xprev)
            position[1] = position[1] + (ydif - yprev)
            xprev = xdif
            yprev = ydif

        except IndexError:
            end()
    

def line(length):
    #Creates a straight line of length
    for x in range(length):
        try:
            print(position[1])
            grid[int(position[0])][int(position[1])] = 'O'
            grid[int(position[0] + math.cos(direction))][int(position[1] - math.sin(direction))] = '#'
            grid[int(position[0] - math.cos(direction))][int(position[1] + math.sin(direction))] = '#'
            position[0] = position[0] + math.sin(direction)
            position[1] = position[1] + math.cos(direction)
        except IndexError:
            end()

def thinline(length):
    #Creates a thin line of length
    for x in range(length):
        try:
            grid[int(position[0])][int(position[1])] = 'O'
            position[0] = position[0] + math.sin(direction)
            position[1] = position[1] + math.cos(direction)
        except IndexError:
            end()

def end():
    for x in range(GRIDSIZE):
        print(grid[x])
    sys.exit()

main()
