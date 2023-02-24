from tkinter import *
from random import *
from tkinter import simpledialog
from tkinter import messagebox
import os
import io
from PIL import Image, ImageTk

from shapely.ops import unary_union
from shapely.geometry import Polygon

D_Points = {}
D_Provinces = {}
D_Pays = {}
D_Drapeaux = {}
D_Formables = {}

currentID = "FRA"
currentPROVID = "p1"
currentFORMABLEID = "BEN"



MODE_DRAWING = 0
MODE_COUNTRY = 1
MODE_PROVINCE = 2
MODE_MERGE = 3
MODE_FORMABLE = 4

currentMOD = MODE_DRAWING

safeMode = False


# ----------------



fenmap = Tk()
fenmap.title("GSG - Europe Editor")


Frame_Info = Frame(fenmap,highlightthickness = 1,highlightbackground="black",height=700,width=200)
Frame_Info.pack(side="left")
Frame_Info.pack_propagate(0) 
canvasmap = Canvas(fenmap,height=700,width=1000,bg="#007F7F")
canvasmap.pack(side="right")


# ----------------------- Fonctions du menu -------------------------------------


def redrawMap():
    if currentMOD == MODE_DRAWING :
        for pays in D_Pays:
            for prov in D_Pays[pays]["provinces"]:
                canvasmap.itemconfigure(prov,fill='#%02x%02x%02x' % tuple(D_Pays[pays]["color"]))
    elif currentMOD == MODE_COUNTRY:
        for prov in D_Provinces:
            if prov in D_Pays[currentID]["cores"]:
                canvasmap.itemconfigure(prov,fill='#%02x%02x%02x' % tuple(D_Pays[currentID]["color"]))
            else:
                canvasmap.itemconfigure(prov,fill='gray')
    elif currentMOD == MODE_PROVINCE:
        for prov in D_Provinces:
            if prov == currentPROVID:
                canvasmap.itemconfigure(prov,fill='blue')
            elif prov in D_Provinces[currentPROVID]["adjacencies"]:
                canvasmap.itemconfigure(prov,fill='cyan')
            elif len(D_Provinces[prov]["adjacencies"]) == 0:
                canvasmap.itemconfigure(prov,fill='yellow')
            else:
                canvasmap.itemconfigure(prov,fill='gray')
    elif currentMOD == MODE_MERGE:
        for prov in D_Provinces:
            if prov == currentPROVID:
                canvasmap.itemconfigure(prov,fill='blue')
            else:
                canvasmap.itemconfigure(prov,fill='gray')
            


def setMODE_DRAWING():
    global currentMOD
    currentMOD = MODE_DRAWING
    redrawMap()

def setMODE_PROVINCE():
    global currentMOD
    currentMOD = MODE_PROVINCE
    redrawMap()

def setMODE_COUNTRY():
    global currentMOD
    currentMOD = MODE_COUNTRY
    redrawMap()

def setMODE_MERGE():
    global currentMOD
    currentMOD = MODE_MERGE
    redrawMap()

def setMODE_FORMABLE():
    global currentMOD
    currentMOD = MODE_FORMABLE
    redrawMap()

def saveSafe():
    global safeMode
    safeMode = True
    SaveJSON()

def saveNormal():
    global safeMode
    safeMode = False
    SaveJSON()


menuBar = Menu(fenmap)
fenmap['menu'] = menuBar
SM_Mode = Menu(menuBar)
SM_Save = Menu(menuBar)

menuBar.add_cascade(label='Modes', menu=SM_Mode)
SM_Mode.add_command(label='Dessin', command=setMODE_DRAWING)
SM_Mode.add_command(label='Province', command=setMODE_PROVINCE)
SM_Mode.add_command(label='Pays', command=setMODE_COUNTRY)
SM_Mode.add_command(label='Merge', command=setMODE_MERGE)
SM_Mode.add_command(label='Formable', command=setMODE_FORMABLE)

menuBar.add_cascade(label='Sauvegarder', menu=SM_Save)
SM_Save.add_command(label='Safe', command=saveSafe)
SM_Save.add_command(label='Normal', command=saveNormal)


# -------------------- Fonctions pour le déplacement de la carte -----------------


def move_start(event):
    canvasmap.scan_mark(event.x, event.y)
def move_move(event):
    canvasmap.scan_dragto(event.x, event.y, gain=1)

canvasmap.bind("<ButtonPress-1>",move_start)
canvasmap.bind("<B1-Motion>", move_move)


# -------------------- Fonctions Chargement Dictionaires -------------------------

def SaveJSON():
    global D_Points,D_Provinces,D_Pays,D_Drapeaux,D_Formables

    path = "Assets/Resources/JSON/"
    if safeMode:
        path = "EditorOutput/"
    
    with io.open(path+"points.json","w",encoding="utf8") as jsonp_file:

        jsonp_file.write("{\n\t\"points\":[\n")
        i = 1
        for point in D_Points:
            jsonp_file.write("\t\t{\"name\":\""+point+"\",\"x\":"+str(D_Points[point][0])+",\"y\":0.2,\"z\":"+str(-D_Points[point][1])+"}")

            if i != len(D_Points):
                jsonp_file.write(",")
            jsonp_file.write("\n")

            i+=1
        jsonp_file.write("\t]\n}")

    with io.open(path+"provinces.json","w",encoding="utf8") as jsonp_file:

        jsonp_file.write("{\n\t\"provinces\":[\n")

        j = 1
        for province in D_Provinces:
            jsonp_file.write("\t\t{\n")
            jsonp_file.write("\t\t\t\"id\":\""+province[1:]+"\",\n")
            jsonp_file.write("\t\t\t\"name\":\""+D_Provinces[province]["name"]+"\",\n")

            jsonp_file.write("\t\t\t\"adjacencies\":[\n")
            for i in range(len(D_Provinces[province]["adjacencies"])):
                jsonp_file.write("\t\t\t\t\""+str(D_Provinces[province]["adjacencies"][i][1:])+"\"")
                if i != len(D_Provinces[province]["adjacencies"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t],\n")
            
            jsonp_file.write("\t\t\t\"vertices\":[\n")
            for i in range(len(D_Provinces[province]["points"])):
                jsonp_file.write("\t\t\t\t\""+str(D_Provinces[province]["points"][i])+"\"")
                if i != len(D_Provinces[province]["points"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t]\n\t\t}")

            if j != len(D_Provinces):
                jsonp_file.write(",")
            jsonp_file.write("\n")

            j+=1
        jsonp_file.write("\t]\n}")

    with io.open(path+"pays.json","w",encoding="utf8") as jsonp_file:
        
        jsonp_file.write("{\n\t\"pays\":[\n")
        i = 1
        for pays in D_Pays:
            jsonp_file.write("\t\t{\n")
            jsonp_file.write("\t\t\t\"id\":\""+pays+"\",\n")
            jsonp_file.write("\t\t\t\"name\":\""+D_Pays[pays]["name"]+"\",\n")
            jsonp_file.write("\t\t\t\"culture\":\""+D_Pays[pays]["culture"]+"\",\n")
            jsonp_file.write("\t\t\t\"color\":"+str(D_Pays[pays]["color"])+",\n")
            jsonp_file.write("\t\t\t\"secretNation\":\""+D_Pays[pays]["secretNation"]+"\"\n")
            jsonp_file.write("\t\t}")

            if i != len(D_Pays):
                jsonp_file.write(",")
            jsonp_file.write("\n")

            i+=1
        jsonp_file.write("\t]\n}")

    with io.open(path+"history.json","w",encoding="utf8") as jsonp_file:
        
        jsonp_file.write("{\n\t\"pays\":[\n")
        j = 1
        for pays in D_Pays:
            jsonp_file.write("\t\t{\n")
            jsonp_file.write("\t\t\t\"id\":\""+pays+"\",\n")
            jsonp_file.write("\t\t\t\"governement\":"+str(D_Pays[pays]["governement"])+",\n")

            jsonp_file.write("\t\t\t\"parties\":[\n")
            for i in range(5):
                jsonp_file.write("\t\t\t\t{\n")
                jsonp_file.write("\t\t\t\t\t\"name\":\""+D_Pays[pays]["parties"][i]["name"]+"\",\n")
                jsonp_file.write("\t\t\t\t\t\"popularity\":"+str(D_Pays[pays]["parties"][i]["popularity"])+"\n")
                jsonp_file.write("\t\t\t\t}")
                if i != 4:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t],\n")


            jsonp_file.write("\t\t\t\"provinces\":[\n")
            for i in range(len(D_Pays[pays]["provinces"])):
                jsonp_file.write("\t\t\t\t\""+str(D_Pays[pays]["provinces"][i][1:])+"\"")
                if i != len(D_Pays[pays]["provinces"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t],\n")

            jsonp_file.write("\t\t\t\"cores\":[\n")
            for i in range(len(D_Pays[pays]["cores"])):
                jsonp_file.write("\t\t\t\t\""+str(D_Pays[pays]["cores"][i][1:])+"\"")
                if i != len(D_Pays[pays]["cores"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t]\n")
            jsonp_file.write("\t\t}")

            if j != len(D_Pays):
                jsonp_file.write(",")
            jsonp_file.write("\n")
            j+=1
        jsonp_file.write("\t]\n}")


    with io.open(path+"formables.json","w",encoding="utf8") as jsonp_file:
        
        jsonp_file.write("{\n\t\"formables\":[\n")
        j = 1
        for pays in D_Formables:
            jsonp_file.write("\t\t{\n")
            jsonp_file.write("\t\t\t\"id\":\""+pays+"\",\n")
            jsonp_file.write("\t\t\t\"name\":\""+D_Formables[pays]["name"]+"\",\n")


            jsonp_file.write("\t\t\t\"contestants\":[\n")
            for i in range(len(D_Formables[pays]["contestants"])):
                jsonp_file.write("\t\t\t\t\""+D_Formables[pays]["contestants"][i]+"\"")
                if i != len(D_Formables[pays]["contestants"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t],\n")

            jsonp_file.write("\t\t\t\"required\":[\n")
            for i in range(len(D_Formables[pays]["required"])):
                jsonp_file.write("\t\t\t\t\""+str(D_Formables[pays]["required"][i][1:])+"\"")
                if i != len(D_Formables[pays]["required"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t]\n")
            jsonp_file.write("\t\t}")

            if j != len(D_Formables):
                jsonp_file.write(",")
            jsonp_file.write("\n")
            j+=1
        jsonp_file.write("\t]\n}")
            

def LoadJSONs():
    global D_Points,D_Provinces,D_Pays,D_Drapeaux,D_Formables
    with io.open("Assets/Resources/JSON/points.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["points"]
        for point in json:
            D_Points[point["name"]] = [point["x"],-point["z"]]
        
    with io.open("Assets/Resources/JSON/provinces.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["provinces"]
        
        for province in json:
            D_Provinces["p"+province["id"]] = {}
            D_Provinces["p"+province["id"]]["name"] = province["name"]
            D_Provinces["p"+province["id"]]["owner"] = "000"
            D_Provinces["p"+province["id"]]["points"] = province["vertices"]
            D_Provinces["p"+province["id"]]["adjacencies"] = ["p"+adjacent for adjacent in province["adjacencies"]]
            listPoints = [D_Points[point] for point in province["vertices"]] 
            canvasmap.create_polygon(listPoints, outline='black',fill="gray", width=1,tags="p"+province["id"])
            canvasmap.tag_bind("p"+province["id"],"<Button-3>",RightClick)
            canvasmap.tag_bind("p"+province["id"],"<Button-1>",LeftClick)

    with io.open("Assets/Resources/JSON/pays.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["pays"]
        for pays in json:
            D_Pays[pays["id"]] = {}
            D_Pays[pays["id"]]["name"] = pays["name"]
            D_Pays[pays["id"]]["color"] = pays["color"]
            D_Pays[pays["id"]]["culture"] = pays["culture"]
            D_Pays[pays["id"]]["secretNation"] = pays["secretNation"]
          
    with io.open("Assets/Resources/JSON/history.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["pays"]
        for pays in json:
            D_Pays[pays["id"]]["governement"] = pays["governement"]
            D_Pays[pays["id"]]["parties"] = pays["parties"]
            D_Pays[pays["id"]]["provinces"] = ["p"+province for province in pays["provinces"]]
            D_Pays[pays["id"]]["cores"] = ["p"+core for core in pays["cores"]]

            for i in range(len(pays["provinces"])):
                D_Provinces["p"+pays["provinces"][i]]["owner"] = pays["id"]
                canvasmap.itemconfigure("p"+pays["provinces"][i],fill='#%02x%02x%02x' % tuple(D_Pays[pays["id"]]["color"]))

    for Pays in D_Pays:
        D_Drapeaux[Pays] = {}
        for Parti in ["republic","monarchy","communism","fascism"]:
            Path = "Assets/Resources/Flags/"+Pays+"_"+Parti+".png"
            if not os.path.exists(Path):
                Path = Path = "Assets/Resources/Flags/"+Pays+".png"
            D_Drapeaux[Pays][Parti] = ImageTk.PhotoImage(Image.open(Path).resize((200, 150)))


    with io.open("Assets/Resources/JSON/formables.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["formables"]
        for formable in json:
            D_Formables[formable["id"]] = {}
            D_Formables[formable["id"]]["name"] = formable["name"]
            D_Formables[formable["id"]]["contestants"] = formable["contestants"]
            D_Formables[formable["id"]]["required"] = ["p"+province for province in formable["required"]]


def RightClick(event):
    global D_Pays,D_Provinces,currentMOD
    idNum = event.widget.gettags('current')[0]

    if currentMOD == MODE_DRAWING:
        D_Pays[D_Provinces[idNum]["owner"]]["provinces"].remove(idNum)
        D_Pays[currentID]["provinces"].append(idNum)
        D_Provinces[idNum]["owner"] = currentID
        canvasmap.itemconfigure(idNum,fill='#%02x%02x%02x' % tuple(D_Pays[currentID]["color"]))
    elif currentMOD == MODE_PROVINCE and idNum != currentPROVID:
        if idNum in D_Provinces[currentPROVID]["adjacencies"]:
            D_Provinces[currentPROVID]["adjacencies"].remove(idNum)
            canvasmap.itemconfigure(idNum,fill='gray')
        else:
            D_Provinces[currentPROVID]["adjacencies"].append(idNum)
            canvasmap.itemconfigure(idNum,fill='cyan')
    elif currentMOD == MODE_COUNTRY:
        if idNum in D_Pays[currentID]["cores"]:
            D_Pays[currentID]["cores"].remove(idNum)
            canvasmap.itemconfigure(idNum,fill='gray')
        else:
            D_Pays[currentID]["cores"].append(idNum)
            canvasmap.itemconfigure(idNum,fill='#%02x%02x%02x' % tuple(D_Pays[currentID]["color"]))
    elif currentMOD == MODE_MERGE:
        FusePolygons(currentPROVID,idNum)

            
def LeftClick(event):
    global currentPROVID,currentMOD
    idNum = event.widget.gettags('current')[0]

    if currentMOD in [MODE_PROVINCE,MODE_MERGE]:
        currentPROVID = idNum
        redrawMap()

def WipeInfo():
    for element in Frame_Info.winfo_children():
        element.destroy()
        
        
def FusePolygons(poly1,poly2):
    global D_Provinces,D_Pays,D_Formables
    pos1 = [ D_Points[point] for point in D_Provinces[poly1]["points"]]
    pos2 = [ D_Points[point] for point in D_Provinces[poly2]["points"]]

    names = D_Provinces[poly1]["points"] + D_Provinces[poly2]["points"]

    polys = [Polygon(pos1),Polygon(pos2)]

    mergedPolys = unary_union(polys)

    l = []
    for coord in mergedPolys.boundary.coords:
        for name in names:
            if coord[0] == D_Points[name][0] and coord[1] == D_Points[name][1]:
                l.append(name)
                break


    canvasmap.delete(poly1)
    canvasmap.delete(poly2)

    D_Pays[D_Provinces[poly2]["owner"]]["provinces"].remove(poly2)
    D_Provinces[poly1]["points"] = l

    if poly2 in D_Provinces[poly1]["adjacencies"]:
        D_Provinces[poly1]["adjacencies"].remove(poly2)

    for adjacent in D_Provinces[poly2]["adjacencies"]:
        if not adjacent in D_Provinces[poly1]["adjacencies"]:
            D_Provinces[poly1]["adjacencies"].append(adjacent)

    del D_Provinces[poly2]

    for formable in D_Formables:
        if poly2 in D_Formables[formable]["required"]:
            D_Formables[formable]["required"].remove(poly2)

    for pays in D_Pays:
        if poly2 in D_Pays[pays]["cores"]:
            D_Pays[pays]["cores"].remove(poly2)

    poly = canvasmap.create_polygon(list(mergedPolys.boundary.coords), outline='black',fill="blue", width=1, tags=poly1)

    canvasmap.tag_bind(poly1,"<Button-3>",RightClick)
    canvasmap.tag_bind(poly1,"<Button-1>",LeftClick)
    
    

# ----------------------- Fonctions relatives à la carte ------------------


def CenterPolygon(Coords):
    x = 0
    y = 0
    for i in range(len(Coords)):
        if i%2==0:
            x = x + Coords[i]
        else:
            y = y + Coords[i]
    x = x/(len(Coords)/2)
    y = y/(len(Coords)/2)
    return (x,y)


def convertIntToGovernement(i):
    if i <= 2:
        return "republic"
    if i <= 5:
        return "monarchy"
    if i <= 8:
        return "communism"
    return "fascism"

#--------------------------------------




LoadJSONs()

# ------------------------ GUI -------------------------------

def ChangeCurrentID(newVal):
    global currentID,Pays_Flag,governement

    for key in D_Pays:
        if newVal == D_Pays[key]["name"]:
            currentID = key
            governement.set(D_Pays[currentID]["governement"])
            Pays_Flag.configure(image=D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])])
            Pays_Flag.image = D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])]
            redrawMap()
            return

def ChangeCountryInfos():
    global Pays_Flag, D_Pays

    D_Pays[currentID]["governement"] = governement.get()
    Pays_Flag.configure(image=D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])])
    Pays_Flag.image = D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])]

idPays = StringVar()
governement = IntVar()

liste = []
for key in D_Pays:
    liste.append(D_Pays[key]["name"])

liste.sort()

for key in D_Pays:
    if D_Pays[key]["name"] == liste[0]:
        currentID = key
        break
idPays.set(liste[0])
options = OptionMenu(Frame_Info, idPays, *liste,command=ChangeCurrentID)
options.pack()

Pays_Flag = Label(Frame_Info,image=D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])]) #Info Pays
Pays_Flag.pack()

governement.set(D_Pays[currentID]["governement"])
entryGov = Entry(Frame_Info,textvariable=governement)
entryGov.pack()

boutonGov = Button(Frame_Info,text="Valid Changes",command=ChangeCountryInfos)
boutonGov.pack()

fenmap.mainloop()
