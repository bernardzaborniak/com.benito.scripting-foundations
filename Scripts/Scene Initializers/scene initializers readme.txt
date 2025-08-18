Scene Initializers are similar to Scene Managers, but their purpose is different, 
theyre only used for different initialization logic that can be set differently per scene.
They are also singletons , but their execution order is after the Local Scene Singletons

When preloading scenes, how do we make sure not to have 2 of them at the same time?