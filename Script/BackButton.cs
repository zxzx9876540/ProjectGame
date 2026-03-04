using Godot;
using System;
//ประกาศคลาสแบบ public ชื่อ BackButton
public partial class BackButton : Button
{
	//เขียนทับฟังก์ชันเดิมของ Button
	public override void _Ready()
	{
		//ถ้ามีคนกดปุ่มให้เรียกฟังก์ชัน onclick
		Pressed += onclick;
	}
	//สร้างฟังก์ชันชื่อ onclick
	private void onclick()
	{
		//เมื่อมีการเรียกใช้ onclick ให้ทำการเปลี่ยน scene
		GetTree().ChangeSceneToFile("res://CreateCharacter.tscn");
	}
}
