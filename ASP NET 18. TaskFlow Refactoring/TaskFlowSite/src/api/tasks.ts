import {apiFetch} from "./client";
import type {TaskItem} from "../types/api";

export async function getTasksByProject(projectId: number){
    const res = await apiFetch<TaskItem[]>(
        `/api/taskitems/project/${projectId}`,
    )
    if(!res.ok || !res.data?.data) return{success:false, data: [] as TaskItem[]};
    return {success:true, data: res.data.data}
}

export async function getTask(id: number){
    const res = await apiFetch<TaskItem>(
        `/api/taskitems/${id}`,
    )
    if(!res.ok || !res.data?.data) return{success:false, data: null};
    return {success:true, data: res.data.data}
}

export async function createTask(
    projectId:number,
    title: string,
    description?:string,
    priority = "Medium")  {
    const  res
        = await apiFetch<TaskItem>(
        `/api/taskitems/`,
        {
            method: "POST",
            body: JSON.stringify({
                projectId,
                title, description:description??null,
                priority,
            }),
        });

    if(!res.ok || !res.data?.data)
        return{
            success:false,
            error: res.problem?.detail ?? res.data?.message,
            status:res.status,
        };
    return {success:true, data: res.data.data}
}

export async function updateTask(
    id:number,
    payload:{
        title: string,
        description?:string,
        status:string,
        priority:string
    }
    )  {
    const  res
        = await apiFetch<TaskItem>(
        `/api/taskitems/${id}`,
        {
            method: "PUT",
            body: JSON.stringify(payload),
        });

    if(!res.ok || !res.data?.data)
        return{
            success:false,
            error: res.problem?.detail ?? res.data?.message,
            status:res.status,
        };
    return {success:true, data: res.data.data}
}

export async function deleteTask(id: number) {
    const res = await apiFetch(
        `/api/taskitems/${id}`,
        {method: "DELETE"});
    return {ok: res.ok, status: res.status};
}
