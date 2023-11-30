from tkinter import *
from random import *
from tkinter import simpledialog
from tkinter import messagebox
from tkinter import colorchooser
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
currentPROVID = "p2"
currentFORMABLEID = "BEN"


scaleFactor = 1


MODE_DRAWING = 0
MODE_COUNTRY = 1
MODE_PROVINCE = 2
MODE_MERGE = 3
MODE_FORMABLE = 4

currentMOD = MODE_DRAWING
labelsHidden = False

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
    elif currentMOD == MODE_FORMABLE:
        for prov in D_Provinces:
            if prov in D_Formables[currentFORMABLEID]["required"]:
                canvasmap.itemconfigure(prov,fill='blue')
            else:
                canvasmap.itemconfigure(prov,fill='gray')
            


def setMODE_DRAWING():
    global currentMOD
    currentMOD = MODE_DRAWING
    SetCountryMODE()
    redrawMap()

def setMODE_PROVINCE():
    global currentMOD
    SetProvinceMODE()
    currentMOD = MODE_PROVINCE
    redrawMap()

def setMODE_COUNTRY():
    global currentMOD
    currentMOD = MODE_COUNTRY
    SetCountryMODE()
    redrawMap()

def setMODE_MERGE():
    global currentMOD
    currentMOD = MODE_MERGE
    WipeInfo()
    redrawMap()

def setMODE_FORMABLE():
    global currentMOD
    currentMOD = MODE_FORMABLE
    SetFormableMODE()
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

def zoom(event):
    global scaleFactor
    x = canvasmap.canvasx(event.x)
    y = canvasmap.canvasy(event.y)


    if  event.delta == -120:  # scroll down
        scaleFactor = 0.5
    if event.delta == 120:  # scroll up
        scaleFactor = 1.5

    
    canvasmap.scale('all', x, y, scaleFactor, scaleFactor)

def hide(event):
    global labelsHidden

    labelsHidden = not labelsHidden
    
    state = "normal"
    if labelsHidden:
        state = "hidden"
        
    canvasmap.itemconfigure("points", state=state)

canvasmap.bind("<ButtonPress-1>",move_start)
canvasmap.bind("<B1-Motion>", move_move)
canvasmap.bind('<MouseWheel>', zoom)
canvasmap.bind_all("<h>",hide)


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
            jsonp_file.write("\t\t{\"id\": "+str(point)+",\"x\":"+str(D_Points[point][0])+",\"y\":0.2,\"z\":"+str(-D_Points[point][1])+"}")

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
            jsonp_file.write("\t\t\t\"id\":"+str(province[1:])+",\n")
            jsonp_file.write("\t\t\t\"name\":\""+D_Provinces[province]["name"]+"\",\n")
            jsonp_file.write("\t\t\t\"type\":\""+D_Provinces[province]["type"]+"\",\n")

            jsonp_file.write("\t\t\t\"adjacencies\":[\n")
            for i in range(len(D_Provinces[province]["adjacencies"])):
                jsonp_file.write("\t\t\t\t"+str(D_Provinces[province]["adjacencies"][i][1:]))
                if i != len(D_Provinces[province]["adjacencies"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t],\n")
            
            jsonp_file.write("\t\t\t\"vertices\":[\n")
            for i in range(len(D_Provinces[province]["points"])):
                jsonp_file.write("\t\t\t\t"+str(D_Provinces[province]["points"][i]))
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
                jsonp_file.write("\t\t\t\t"+str(D_Pays[pays]["provinces"][i][1:]))
                if i != len(D_Pays[pays]["provinces"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t],\n")

            jsonp_file.write("\t\t\t\"cores\":[\n")
            for i in range(len(D_Pays[pays]["cores"])):
                jsonp_file.write("\t\t\t\t"+str(D_Pays[pays]["cores"][i][1:]))
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
                jsonp_file.write("\t\t\t\t"+str(D_Formables[pays]["required"][i][1:]))
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
            D_Points[point["id"]] = [point["x"],-point["z"]]
        
    with io.open("Assets/Resources/JSON/provinces.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["provinces"]
        
        for province in json:
            idProvTemp = "p" + str(province["id"])
            D_Provinces[idProvTemp] = {}
            D_Provinces[idProvTemp]["name"] = province["name"]
            D_Provinces[idProvTemp]["owner"] = "000"
            D_Provinces[idProvTemp]["type"] = province["type"]
            D_Provinces[idProvTemp]["points"] = province["vertices"]
            D_Provinces[idProvTemp]["adjacencies"] = ["p"+str(adjacent) for adjacent in province["adjacencies"]]
            listPoints = [D_Points[point] for point in province["vertices"]]
            canvasmap.create_polygon(listPoints, outline='black',fill="gray", width=1,tags=idProvTemp)
            canvasmap.tag_bind(idProvTemp,"<Button-3>",RightClick)
            canvasmap.tag_bind(idProvTemp,"<Button-1>",LeftClick)

    for point in D_Points:
        canvasmap.create_text(D_Points[point], text=str(point), fill="black", font=('Helvetica 16 bold'),state="hidden",tags="points")


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
            D_Pays[pays["id"]]["provinces"] = ["p"+str(province) for province in pays["provinces"]]
            D_Pays[pays["id"]]["cores"] = ["p"+str(core) for core in pays["cores"]]

            for i in range(len(pays["provinces"])):
                D_Provinces["p"+str(pays["provinces"][i])]["owner"] = pays["id"]
                canvasmap.itemconfigure("p"+str(pays["provinces"][i]),fill='#%02x%02x%02x' % tuple(D_Pays[pays["id"]]["color"]))

    with io.open("Assets/Resources/JSON/formables.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["formables"]
        for formable in json:
            D_Formables[formable["id"]] = {}
            D_Formables[formable["id"]]["name"] = formable["name"]
            D_Formables[formable["id"]]["contestants"] = formable["contestants"]
            D_Formables[formable["id"]]["required"] = ["p"+str(province) for province in formable["required"]]


    for Pays in D_Pays:
        D_Drapeaux[Pays] = {}
        for Parti in ["republic","monarchy","communism","fascism"]:
            Path = "Assets/Resources/Flags/"+Pays+"_"+Parti+".png"
            if not os.path.exists(Path):
                Path = Path = "Assets/Resources/Flags/"+Pays+".png"
            D_Drapeaux[Pays][Parti] = ImageTk.PhotoImage(Image.open(Path).resize((200, 150)))


    for Pays in D_Formables:
        D_Drapeaux[Pays] = {}
        for Parti in ["republic","monarchy","communism","fascism"]:
            Path = "Assets/Resources/Flags/"+Pays+"_"+Parti+".png"
            if not os.path.exists(Path):
                Path = Path = "Assets/Resources/Flags/"+Pays+".png"
            D_Drapeaux[Pays][Parti] = ImageTk.PhotoImage(Image.open(Path).resize((200, 150)))



def RightClick(event):
    global D_Pays,D_Provinces,D_Formables,currentMOD,currentFORMABLEID
    idNum = event.widget.gettags('current')[0]

    if currentMOD == MODE_DRAWING:
        D_Pays[D_Provinces[idNum]["owner"]]["provinces"].remove(idNum)
        D_Pays[currentID]["provinces"].append(idNum)
        D_Provinces[idNum]["owner"] = currentID
        canvasmap.itemconfigure(idNum,fill='#%02x%02x%02x' % tuple(D_Pays[currentID]["color"]))
    elif currentMOD == MODE_PROVINCE and idNum != currentPROVID:
        if idNum == currentPROVID:
            return
        
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
    elif currentMOD == MODE_FORMABLE:
        if idNum in D_Formables[currentFORMABLEID]["required"]:
            D_Formables[currentFORMABLEID]["required"].remove(idNum)
            canvasmap.itemconfigure(idNum,fill='gray')
        else:
            D_Formables[currentFORMABLEID]["required"].append(idNum)
            canvasmap.itemconfigure(idNum,fill='blue')

            
def LeftClick(event):
    global currentPROVID,currentMOD
    idNum = event.widget.gettags('current')[0]

    if currentMOD in [MODE_PROVINCE,MODE_MERGE]:
        currentPROVID = idNum

        if currentMOD == MODE_PROVINCE:
            SetProvinceMODE()
        redrawMap()

def WipeInfo():
    for element in Frame_Info.winfo_children():
        element.destroy()
        
        
def FusePolygons(poly1,poly2):
    if poly1 == poly2:
        return
    
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
        if poly2 in D_Provinces[adjacent]["adjacencies"]:
            D_Provinces[adjacent]["adjacencies"].remove(poly2)
        if not poly1 in D_Provinces[adjacent]["adjacencies"]:
            D_Provinces[adjacent]["adjacencies"].append(poly1)
            
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




def SetCountryMODE():
    WipeInfo()

    listeGov = ["Parlimentary Rep.","Mixed Rep.","Presidential Rep.","Absolute Mon.","Elective Mon.","Parlimentary Mon.","Soviet Rep.","People's Rep.","Popular Union","New Reich","Social Republic","Military Junta"]
            

    idPays = StringVar()
    countryName = StringVar()
    cultureName = StringVar()
    governement = StringVar()
    varCheckBox = IntVar()

    def ChangeCurrentID(newVal):
        global currentID

        for key in D_Pays:
            if newVal == D_Pays[key]["name"]:
                currentID = key

                if D_Pays[currentID]["secretNation"] == "False":
                    varCheckBox.set(0)
                else:
                    varCheckBox.set(1)
                
                governement.set(listeGov[D_Pays[currentID]["governement"]])
                countryName.set(D_Pays[key]["name"])
                cultureName.set(D_Pays[key]["culture"])
                Pays_Flag.configure(image=D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])])
                Pays_Flag.image = D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])]
                redrawMap()
                return
            

    def ChangeCountryInfos():
        global D_Pays

        D_Pays[currentID]["governement"] = listeGov.index(governement.get())
        D_Pays[currentID]["secretNation"] = ["False","True"][varCheckBox.get()]
        D_Pays[currentID]["name"] = countryName.get()
        D_Pays[currentID]["culture"] = cultureName.get()
        
        Pays_Flag.configure(image=D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])])
        Pays_Flag.image = D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])]
        SetCountryMODE()
        
        redrawMap()


    


    liste = []
    for key in D_Pays:
        liste.append(D_Pays[key]["name"])

    liste.sort()

        
    idPays.set(D_Pays[currentID]["name"])
    
    optionsPays = OptionMenu(Frame_Info, idPays, *liste,command=ChangeCurrentID)
    optionsPays.pack()

    Pays_Flag = Label(Frame_Info,image=D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])]) #Info Pays
    Pays_Flag.pack()

    countryName.set(D_Pays[currentID]["name"])
    entryName = Entry(Frame_Info,textvariable=countryName)
    entryName.pack()

    cultureName.set(D_Pays[currentID]["culture"])
    entryCulture = Entry(Frame_Info,textvariable=cultureName)
    entryCulture.pack()

    governement.set(listeGov[D_Pays[currentID]["governement"]])
    optionGov = OptionMenu(Frame_Info, governement, *listeGov)
    optionGov.pack()

    def choose_color():
        global D_Pays
        color = colorchooser.askcolor(title ="Choose color")[0]
        D_Pays[currentID]["color"] = list(color)
        
        redrawMap()

    buttonColor = Button(Frame_Info, text = "Change color",
                   command = choose_color)
    buttonColor.pack()


    secretNationBox = Checkbutton(Frame_Info, text='Secret Nation',variable=varCheckBox, onvalue=1, offvalue=0)
    secretNationBox.pack()


    boutonGov = Button(Frame_Info,text="Valid Changes",command=ChangeCountryInfos)
    boutonGov.pack()



def SetProvinceMODE():
    WipeInfo() 

    provName = StringVar()
    provType = StringVar()
            

    def ChangeProvinceInfos():
        global D_Provinces

        D_Provinces[currentPROVID]["name"] = provName.get()
        D_Provinces[currentPROVID]["type"] = provType.get()


    provName.set(D_Provinces[currentPROVID]["name"])
    provType.set(D_Provinces[currentPROVID]["type"])
    entryName = Entry(Frame_Info,textvariable=provName)
    entryName.pack()

    liste = ["NORMAL","COSTAL","SEA"]

    optionType = OptionMenu(Frame_Info, provType, *liste)
    optionType.pack()

    boutonValid = Button(Frame_Info,text="Valid Changes",command=ChangeProvinceInfos)
    boutonValid.pack()


def SetFormableMODE():
    WipeInfo()
      

    idPays = StringVar()
    idRemove = StringVar()
    newTag = StringVar()
    countryName = StringVar()

    def ChangeCurrentID(newVal):
        global currentFORMABLEID

        for key in D_Formables:
            if newVal == D_Formables[key]["name"]:
                currentFORMABLEID = key

                countryName.set(D_Formables[key]["name"])

                Pays_Flag.configure(image=D_Drapeaux[currentFORMABLEID]["republic"])
                Pays_Flag.image = D_Drapeaux[currentFORMABLEID]["republic"]
                redrawMap()
                return
            

    def ChangeFormableInfos():
        global D_Formables

        D_Formables[currentFORMABLEID]["name"] = countryName.get()
        
        SetFormableMODE()
        redrawMap()


    


    liste = []
    for key in D_Formables:
        liste.append(D_Formables[key]["name"])

    liste.sort()

        
    idPays.set(D_Formables[currentFORMABLEID]["name"])
    
    optionsPays = OptionMenu(Frame_Info, idPays, *liste,command=ChangeCurrentID)
    optionsPays.pack()

    Pays_Flag = Label(Frame_Info,image=D_Drapeaux[currentFORMABLEID]["republic"]) #Info Pays
    Pays_Flag.pack()

    countryName.set(D_Formables[currentFORMABLEID]["name"])
    entryName = Entry(Frame_Info,textvariable=countryName)
    entryName.pack()

    
    def addNewContestant():
        global D_Formables
        
        if not newTag.get() in D_Formables[currentFORMABLEID]["contestants"]:
            D_Formables[currentFORMABLEID]["contestants"].append(newTag.get())
            SetFormableMODE()
            
            
    def removeContestant():
        global D_Formables
        
        for key in D_Formables[currentFORMABLEID]["contestants"]:
            if D_Pays[key]["name"] == idRemove.get():
                D_Formables[currentFORMABLEID]["contestants"].remove(key)
                SetFormableMODE()
                return


            
    listeC = [ D_Pays[key]["name"] for key in D_Formables[currentFORMABLEID]["contestants"] ]

    listeC.sort()

    idRemove.set(listeC[0])
    
    optionsRemove = OptionMenu(Frame_Info, idRemove, *listeC)
    optionsRemove.pack()
    
    boutonRemove = Button(Frame_Info,text="Remove Contestant",command=removeContestant)
    boutonRemove.pack()
    
    newTag.set("")
    entryNewTag = Entry(Frame_Info,textvariable=newTag)
    entryNewTag.pack()
    
    boutonNew = Button(Frame_Info,text="Add Contestant",command=addNewContestant)
    boutonNew.pack()

    boutonGov = Button(Frame_Info,text="Valid Changes",command=ChangeFormableInfos)
    boutonGov.pack()


SetCountryMODE()



fenmap.mainloop()
