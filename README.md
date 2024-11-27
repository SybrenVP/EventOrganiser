# Event Organizer
Event Organizer is a Unity tool designed to improve the Unity Event dropdown experience. It allows for organizing methods in the Unity Event Dropdown, making it easier to find and manage your event-driven logic. 

## Features
**Group-based organization**: Groups public methods under their respective group (defined in the Event attribute) in the Unity Event dropdown.  
**Order-based organization**: Order public methods by providing an order id in the Event attribute.  
**Improved Workflow**: Say goodbye to the frustration of scrolling through an unorganized list of methods.  
**Team Collaboration**: Allows team members to find your labeled methods more easily. 
**Simple Integration**: Lightweight and easy to add to your Unity project.  
## Installation
Unity version 2022.3.35f1 and higher.  

## Usage
On any public method you can add an attribute `[SVP.Events.Event]`  

### Group parameter  
Adding the group parameter to the event attribute will separate all methods within that group in the dropdown.
This can be useful if you want to section parts of the functionality in separate groups for team members to find what they need more efficiently.
<p align ="center">
<img src="https://github.com/user-attachments/assets/3f10bdb3-f8cc-45b3-8d95-a35937a3d3b1"/>
<img src="https://github.com/user-attachments/assets/605c0a40-ad70-4889-a0d5-823b91f7075e"/>
</p>

### Order parameter  
Adding the order parameter to the event attribute will order the methods based on the order id you give them.
This can be useful if there's that one method you or a team member needs a thousand times, so you can put it on top of everything making it very easy to find.  
<p align ="center">
<img src="https://github.com/user-attachments/assets/7c2b2b3f-26a9-487e-8ceb-8c3c48a10604"/>
</p>

#### Before
![image](https://github.com/user-attachments/assets/32ce2d1d-a786-4508-9c5e-78f934db988c)
#### After
![image](https://github.com/user-attachments/assets/bfdd9e27-cb10-42cd-bb0d-0b1d612eee15)


## Requirements
Unity 2021.1 or higher (untested on previous versions)
## Contributions
Feedback and contributions are welcome! If you encounter a bug or have ideas for enhancements, please open an issue or submit a pull request.

## Support & Feedback
I’d love to hear your thoughts or feature requests! Feel free to reach out or open a discussion in the repository.

## License
This project is licensed under the MIT License.
