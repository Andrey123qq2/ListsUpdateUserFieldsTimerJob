# Description
SharePoint TimerJob for updating the lists fields based on the account attributes of the user field.

### Features:
1. Each list contains individual settings (see in list settings)
1. TimerJob has global settings page and page with list of all lists settings
1. In default mode TimerJob updates attribute fields based on attribute changes in the profile service (last changes in the 2 days - by default). Such logic is optimal in terms of load when processing a large number of lists with a large number of elements (thousands).
Aditionally can be used caml query for items filtration (see below).
1. In force update mode TimerJob updates all list items. See parameters with the text "Force update *").
1. While processing each list item it blocks the launch of any event handlers - it is necessary to eliminate the arbitrary launch of workflows, notifications etc.
1. Updates items permissions with another solution (ListsUpdatePermissions EventHandler)
1. Creates report after all lists processing
1. Errors are logged to the Application system log, as well as to ULS (lines with the text "Custom TimerJob exception")
1. When adding/importing new users to the profile service all their attributes are automatically included in the new changes
1. When lookup fields are updated the source lists are searched by the text of the attribute value


### CamlQuery examples (the <Where> tag does not need to be specified - it is inserted automatically):
Example 1. Items with "Status" field that is not Completed/Rejected
```
<And>
<Neq><FieldRef Name = 'Status' /> <Value Type = 'Choice'>Completed</Value></Neq>
<Neq><FieldRef Name = 'Status' /> <Value Type = 'Choice'>Rejected</Value></Neq>
</And>
```
Example 2. Items created after a certain date.
```
<Geq><FieldRef Name = Created/><Value IncludeTimeValue = 'FALSE' Type = 'DateTime'>2021-05-01T00: 00: 00Z </Value></Geq>
```
Example 3. Items created in the last day.
```
<Geq><FieldRef Name = Created /> <Value Type = 'DateTime'> <Today OffsetDays = -1 /></Value></Geq>
```