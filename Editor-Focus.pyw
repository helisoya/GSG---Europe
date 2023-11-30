from tkinter import *
from random import *
from tkinter import simpledialog
from tkinter import messagebox
from tkinter import colorchooser
import os
import io
from PIL import Image, ImageTk


focus = {
    "id": "England",
    "focus":[]
    }


safeMode = False


# ----------------



fenmap = Tk()
fenmap.title("GSG - Europe Editor (Focuses)")


Frame_Info = Frame(fenmap,highlightthickness = 1,highlightbackground="black",height=700,width=200)
Frame_Info.pack(side="left")
Frame_Info.pack_propagate(0) 
grid = Frame(fenmap,highlightthickness = 1,highlightbackground="black",height=700,width=1000)
grid.pack(side="right")


for x in range(10):
    for y in range(10):
        b = Button(grid, text ="---", command = click())
        b.grid(x=x,y=y)




# ----------------------- Fonctions du menu -------------------------------------


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
SM_Save = Menu(menuBar)

menuBar.add_cascade(label='Sauvegarder', menu=SM_Save)
SM_Save.add_command(label='Safe', command=saveSafe)
SM_Save.add_command(label='Normal', command=saveNormal)


# -------------------- Fonctions Chargement Dictionaires -------------------------

def LoadJSON(text):
    global focus

    with io.open("Assets/Resources/JSON/Focuses/"+text+".json","r",encoding="utf8") as jsonp_file:
        focus = eval(jsonp_file.read())
    



def SaveJSON():
    global focus

    path = "Assets/Resources/JSON/focus"
    if safeMode:
        path = "EditorOutput/focus/"
    
    with io.open(path+focus["id"]+".json","w",encoding="utf8") as jsonp_file:
        jsonp_file.write("{\n\t\"id\": \""+focus["id"]+"\",")
        jsonp_file.write("\n\t\"focus\":[\n")

        for k in range(len(focus["focus"])):

            f = focus["focus"][k]
            jsonp_file.write("\t\t{\n")
            jsonp_file.write("\t\t\t\"id\": \""+f["id"]+"\",\n")
            jsonp_file.write("\t\t\t\"x\": "+str(f["x"])+",\n")
            jsonp_file.write("\t\t\t\"y\": "+str(f["y"])+",\n")
            jsonp_file.write("\t\t\t\"name\": \""+f["name"]+"\",\n")
            jsonp_file.write("\t\t\t\"desc\": \""+f["desc"]+"\",\n")

            jsonp_file.write("\t\t\t\"required\": [")
            if len(f["required"]) == 0:
                jsonp_file.write("],\n")
            else:
                jsonp_file.write("\n")
                for i in range(len(f["required"])):
                    jsonp_file.write("\t\t\t\t\""+f["required"][i]+"\"")
                    if i < len(f["required"])-1:
                        jsonp_file.write(",")
                    jsonp_file.write("\n")
                jsonp_file.write("\t\t\t],\n")

            jsonp_file.write("\t\t\t\"requireAll\": \""+f["requireAll"]+"\",\n")

            jsonp_file.write("\t\t\t\"exclusive\": [")
            if len(f["exclusive"]) == 0:
                jsonp_file.write("],\n")
            else:
                jsonp_file.write("\n")
                for i in range(len(f["exclusive"])):
                    jsonp_file.write("\t\t\t\t\""+f["exclusive"][i]+"\"")
                    if i < len(f["exclusive"])-1:
                        jsonp_file.write(",")
                    jsonp_file.write("\n")
                jsonp_file.write("\t\t\t],\n")

            jsonp_file.write("\t\t\t\"effect\": [")
            if len(f["effect"]) == 0:
                jsonp_file.write("]\n")
            else:
                jsonp_file.write("\n")
                for i in range(len(f["effect"])):
                    jsonp_file.write("\t\t\t\t\""+f["effect"][i]+"\"")
                    if i < len(f["effect"])-1:
                        jsonp_file.write(",")
                    jsonp_file.write("\n")
                jsonp_file.write("\t\t\t]\n")

            jsonp_file.write("\t\t}")
            if k - len(focus["focus"])-1:
                jsonp_file.write(",")
            jsonp_file.write("\n\t]\n}")


            
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
        
    




LoadJSON("england")

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
    
            

    def ChangeProvinceInfos():
        global D_Provinces

        D_Provinces[currentPROVID]["name"] = provName.get()



    provName.set(D_Provinces[currentPROVID]["name"])
    entryName = Entry(Frame_Info,textvariable=provName)
    entryName.pack()

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
