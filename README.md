# DeepBIM
DeepBIM is an AI-augmented BIM framework to enhance BIM workflows. 

### Revit Chatbot Assistant
![diagram-export-2-2-2025-10_39_40-PM](https://github.com/user-attachments/assets/9a69fe86-76e6-4308-bc43-95dfd4981641)
Integrated LLM in Revit that act as an assistant.
At this stage, the chatbot itself is fully functional, but it’s not yet connected to the Revit API—the video demo showcases its potential.

---

### **Features**
✅ **Revit Ribbon Integration** – Adds a custom **DeepBIM button** inside Revit’s UI.  
✅ **Chat Interface** – Allows users to send queries and receive AI-generated responses.  
✅ **Custom API Support** – Users can replace the default API with their own AI endpoint.  

---

### **System Requirements**
**1. Software & Dependencies**
- **Autodesk Revit 2025**
- **.NET 8.0**
- necessary Assemblies and .NET framework (e.g. RevitAPI, RevitAPIUI, Microsoft.Windows.Desktop.App.WPF, etc)

**2. Folder Structure**
```
deepbim/
├── Dependencies/                    # External libraries
├── App.cs                            # Revit entry point (Ribbon button)
├── Command.cs                        # Executes Chat Window
├── ChatWindow.xaml                   # UI layout
├── ChatWindow.xaml.cs                 # Handles chat interactions
├── DeepSeekService.cs                 # API connection handler
└── MarkdownToInlineConverter.cs       # Markdown formatting utility
```

---

### **Installation & Usage**
1. **Clone the repository**:
   ```sh
   git clone https://github.com/your-username/DeepBIM.git
   cd DeepBIM
   ```
2. **Build the project** in Visual Studio.
3. **Place the compiled `.dll` in the Revit Add-ins folder**:
   ```
   %APPDATA%\Autodesk\Revit\Addins\2023\
   ```
4. **Restart Revit** and locate the **DeepBIM button** in the Ribbon.

(more about Revit Plugin: https://www.autodesk.com/support/technical/article/caas/tsarticles/ts/7I2bC1zUr4VjJ3U31uM66K.html)

---

### **API Integration**
**Default API (Author-Only)**
The API endpoint **currently only allows access to the author**. To use your own API, modify `DeepSeekService.cs`:

```csharp
private static readonly string ApiBaseUrl = Environment.GetEnvironmentVariable("DEEPBIM_API_URL") ?? "https://your-api-url.com";
```
Make sure your API **returns a JSON response in the following format**:
```json
{
  "response": "the response"
}
```

---

### **Customization & Configuration**
**1. Change API Endpoint**
- Set an **environment variable**:
  ```sh
  export DEEPBIM_API_URL="https://your-api-url.com"
  ```
- Or modify `DeepSeekService.cs` and **hardcode your API**.

**2. Update Icons & UI**
- Replace `Resources/blue_whale.png` with your own custom icon.
- Modify `ChatWindow.xaml` for UI changes.
