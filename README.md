EverythingThrough
=================

A plugin for Fiddler to change upstream proxys

Uses "X-OverrideGateway" property to easilly and quickly change proxys

Allows to add/remove proxys. 

Because fiddler reuses client connections by default, it will take a while for the existing connection to time out for it to use the new proxy.
Changing 'Reuse client connections' and 'Reuse connections to servers' in fiddler options will prevent this.
