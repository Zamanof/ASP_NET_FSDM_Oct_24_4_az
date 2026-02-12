import {apiFetch} from "./client";
import type {Project} from "../types/api";

export async function getProjects(){
    const res = await apiFetch<Project[]>(
        '/api/projects',
    )
    if(!res.ok || !res.data?.data) return{success:false, data: [] as Project[]};
    return {success:true, data: res.data.data}
}

export async function getProject(id: number){
    const res = await apiFetch<Project>(
        `/api/projects/${id}`,
    )
    if(!res.ok || !res.data?.data) return{success:false, data: null};
    return {success:true, data: res.data.data};

}

export async function createProject(name: string, description?:string)  {
    const  res
        = await apiFetch<Project>(
            `/api/projects`,
        {
            method: "POST",
            body: JSON.stringify({name, description: description??null}),
        });

    if(!res.ok || !res.data?.data)
        return{
        success:false,
        error: res.problem?.detail ?? res.data?.message,
        status:res.status,
        };
    return {success:true, data: res.data.data}

}
export async function updateProject(id: number, name: string, description?:string)  {
    const  res = await apiFetch<Project>(
        `/api/projects/${id}`,
        {
           method: "PUT",
           body: JSON.stringify({name, description: description??null}),
        });
    if(!res.ok || !res.data?.data)
        return{
            success:false,
            error: res.problem?.detail ?? res.data?.message,
            status:res.status,
        };
    return {success:true, data: res.data.data}
}
export async function deleteProject(id: number)  {
    const res = await apiFetch(
        `/api/projects/${id}`,
        {method: "DELETE"});
    return {ok:res.ok, status:res.status};
}
